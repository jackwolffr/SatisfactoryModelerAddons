using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class MultipleMachineDef
    {
        private MultipleMachineDef()
        {
        }

        public MultipleMachineDef(string prefix)
        {
            Prefix = prefix;
        }

        public string FinalName => Prefix + " " + Name;
        public string Name { get; set; } = "";
        public string PartsRatio { get; set; } = "60";
        public bool Default { get; set; } = false;
        public string Prefix { get; } = "";

        public JsonObject Build()
        {
            return new JsonObject
            {
                { "Name", FinalName },
                { "PartsRatio", PartsRatio},
                { "Default", Default }
            };
        }
    }
}
