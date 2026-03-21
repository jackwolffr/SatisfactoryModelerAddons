using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class GameDatas
    {
        public JsonObject Root { get; }
        private JsonArray _machines;
        private JsonArray _parts;
        private JsonArray _recipes;

        private List<string> partNames = [];

        public GameDatas(JsonObject root)
        {
            Root = root;
            _machines = root["Machines"] as JsonArray;
            _parts = root["Parts"] as JsonArray;
            _recipes = root["Recipes"] as JsonArray;
        }

        public void AddMachine(Machine machine)
        {
            Program.ImageMagick(machine.Image, "160x160", machine.Prefix + "_" + machine.Name.Replace(" ", "_") + ".png");
            _machines.Add(machine.Build());
        }

        public void AddPart(Part part)
        {
            if (!partNames.Contains(part.Prefix + "_" + part.Name))
            {
                partNames.Add(part.Prefix + "_" + part.Name);
                Console.WriteLine(part.Prefix + "_" + part.Name + " " + part.Image);
                Program.ImageMagick(part.Image, "80x80", part.Prefix + "_" + part.Name.Replace(" ", "_") + ".png");
                _parts.Add(part.Build());
            }
        }

        public void AddRecipe(Recipe recipe)
        {
            _recipes.Add(recipe.Build());
        }
    }
}