using ImageMagick;
using log4net;
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
        private static ILog _logger = LogManager.GetLogger(typeof(GameDatas));
        public JsonObject Root { get; }
        private readonly JsonArray _machines;
        private readonly JsonArray _multiMachines;
        private readonly JsonArray _parts;
        private readonly JsonArray _recipes;

        private List<string> partNames = [];

        private GameDatas()
        {
            Root = [];
            _machines = [];
            _multiMachines = [];
            _parts = [];
            _recipes = [];
        }
        public GameDatas(JsonObject? root)
        {
            Root = root ?? [];
            _machines = Root["Machines"] as JsonArray ?? [];
            _multiMachines = Root["MultiMachines"] as JsonArray ?? [];
            _parts = Root["Parts"] as JsonArray ?? [];
            _recipes = Root["Recipes"] as JsonArray ?? [];
        }

        public void AddMachine(Machine machine)
        {
            _logger.Info($"add machine {machine.FinalName} {machine.Image}");
            Program.ImageMagick(machine.Image, new MagickGeometry(160, 160), machine.FinalName.Replace(" ", "_") + ".png");
            _machines.Add(machine.Build());
        }
        public void AddMultipleMachine(MultipleMachine machine)
        {
            _logger.Info($"add multiplemachine {machine.FinalName}");
            _multiMachines.Add(machine.Build());
        }

        public void AddPart(Part part)
        {
            if (!partNames.Contains(part.FinalName))
            {
                _logger.Info($"add part {part.FinalName} {part.Image}");
                partNames.Add(part.FinalName);
                Program.ImageMagick(part.Image, new MagickGeometry(80, 80), part.FinalName.Replace(" ", "_") + ".png");
                _parts.Add(part.Build());
            }
        }

        public void AddRecipe(Recipe recipe)
        {
            _logger.Info($"add recipe {recipe.FinalName} ({recipe.Time}s) [" + string.Join(", ", recipe.Ins.Select(i => $"{i.Amount} {i.FinalName}")) + "] => [" + string.Join(", ", recipe.Outs.Select(i => $"{i.Amount} {i.FinalName}")) + "]");
            _recipes.Add(recipe.Build());
        }
    }
}