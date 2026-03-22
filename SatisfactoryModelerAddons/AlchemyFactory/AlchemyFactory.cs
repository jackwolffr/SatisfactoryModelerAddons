
using HtmlAgilityPack;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public partial class AlchemyFactory
    {
        private static ILog _logger = LogManager.GetLogger(typeof(AlchemyFactory));
        string _datasDirname = Path.Combine(Program.GLOBALDATADIRNAME, "AlchemyFactory");
        private readonly string prefix = "AF";

        public void Apply(GameDatas gameDatas)
        {
            _logger.Info("************** Alchemy Factory **************");

            _logger.Info("load tables.json");
            var tables = JsonSerializer.Deserialize<Tables>(File.ReadAllText(Path.Combine(_datasDirname, "tables.json"))) ?? new();
            var devices = Device.FromHTML(_datasDirname);
            foreach ( var device in devices)
            {
                _logger.Info($"device {device.Name} {device.Image} Heat {device.Heat}");
            }

            tables.Devices.UnionWith(devices);

            var rawMaterials = RawMaterial.FromHTML(_datasDirname, tables.Properties);
            foreach (var rawMaterial in rawMaterials)
            {
                _logger.Info($"raw Material {rawMaterial.Name} {rawMaterial.Image} Cost {rawMaterial.Cost}");
            }
            tables.Items.UnionWith(rawMaterials);
            tables.Recipes.UnionWith(RawMaterial.ToRecipes(rawMaterials, tables.Properties));

            var herbs = Herb.FromHTML(_datasDirname);
            foreach (var herb in herbs)
            {
                _logger.Info($"herb {herb.Name} {herb.Image} Nutriment {herb.Nutriment}");
            }
            tables.Items.UnionWith(herbs);

            var fertilizers = Fertilizer.FromHTML(_datasDirname);
            foreach (var fertilizer in fertilizers)
            {
                _logger.Info($"fertilizer {fertilizer.Name} {fertilizer.Image} Nutriment {fertilizer.Nutriment} NutrimentSpeed {fertilizer.NutrimentSpeed}");
            }
            tables.Items.UnionWith(fertilizers);
            tables.Recipes.UnionWith(Fertilizer.ToRecipes(fertilizers, tables.Properties, herbs));

            var fuels = Fuel.FromHTML(_datasDirname);
            tables.Items.UnionWith(fuels);

            tables.Recipes.UnionWith(Fertilizer.ToRecipesWorldTree(tables.WorldTreeRecipes,fertilizers));

            var recipes = Recipe.FromHTML(_datasDirname).Where(r => !tables.ExcludeRecipes.Contains(r.Name)).ToHashSet();
            tables.Items.UnionWith(Recipe.ToItems(recipes));
            tables.Recipes.UnionWith(recipes);
            tables.ApplyHeatIngredient();
            tables.Recipes.UnionWith(Fuel.ToRecipes(fuels,tables.Properties));

            foreach (var item in tables.Items.Where(i=>i.Image!=""))
            {
                gameDatas.AddPart(item.ToPart(prefix, _datasDirname));
            }
            foreach (var device in tables.Devices)
            {
                gameDatas.AddMachine(device.ToMachine(prefix, _datasDirname));
            }
            foreach (var recipe in tables.Recipes)
            {
                gameDatas.AddRecipe(recipe.ToRecipe(prefix));
            }
            _logger.Info("************** End Alchemy Factory **************");
        }


    }
}