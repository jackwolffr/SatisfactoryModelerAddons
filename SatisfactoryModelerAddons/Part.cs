using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class Part
    {
        private Part()
        {
        }
        public Part(string prefix)
        {
            Prefix = prefix;
        }
        public string FinalName => Prefix + " " + Name;
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Tier { get; set; } = "0-2";
        public string SinkPoints { get; set; } = "0";
        public string Prefix { get; } = "";

        public JsonObject Build()
        {
            return new JsonObject
            {
                { "Name", FinalName },
                { "Tier", Tier },
                { "SinkPoints", SinkPoints }
            };
        }
    }
}
