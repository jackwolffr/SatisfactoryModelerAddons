using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons
{
    public class Ingredient
    {
        public Ingredient(string prefix)
        {
            Prefix = prefix;
        }
        public bool IsNeg { get; set; } = false;
        public string Name { get; set; } = "";
        public string Amount { get; set; } = "";
        public string Prefix { get; }

        public JsonObject Build()
        {
            return new JsonObject()
            {
                { "Part", Prefix + " " + Name },
                { "Amount",(IsNeg?"-":"") + Amount }
            };
        }
    }
}
