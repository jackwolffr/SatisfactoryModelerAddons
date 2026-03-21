using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class Recipe
    {
        public Recipe(string prefix)
        {
            Prefix = prefix;
        }

        public string Name { get; set; } = "";
        public string MachineName { get; set; } = "";
        public string Time { get; set; } = "";
        public string Tier { get; set; } = "0-2";
        public List<Ingredient> Ins { get; set; } = [];
        public List<Ingredient> Outs { get; set; } = [];
        public string Prefix { get; }

        public JsonObject Build()
        {
            return new JsonObject()
            {
                { "Name", Prefix + " " + Name },
                { "Machine",  Prefix + " " + MachineName },
                { "BatchTime", Time },
                { "Tier", Tier },
                { "Parts", BuildParts() }
            };
        }

        private JsonArray BuildParts()
        {
            JsonArray array = [];
            foreach (var ingredient in Ins)
            {
                ingredient.IsNeg = true;
                array.Add(ingredient.Build());
            }
            foreach (var ingredient in Outs)
            {
                array.Add(ingredient.Build());
            }

            return array;
        }
    }
}
