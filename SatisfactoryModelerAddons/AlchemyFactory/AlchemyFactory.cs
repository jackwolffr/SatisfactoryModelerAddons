
using HtmlAgilityPack;
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
        string _datasDirname = Program.GLOBALDATADIRNAME + "\\AlchemyFactory\\datas";
        private readonly string prefix = "AF";

        public void Apply(GameDatas gameDatas)
        {
            var tables = JsonSerializer.Deserialize<Tables>(File.ReadAllText(Path.Combine(_datasDirname, "tables.json")));
            tables.Devices.UnionWith(Device.FromHTML(Path.Combine(_datasDirname, "Devices - Alchemy Factory Codex.html"), _datasDirname));
            tables.Recipes.UnionWith(RawMaterial.FromHTML(Path.Combine(_datasDirname, "Raw Materials - Alchemy Factory Codex.html"), tables.Properties));
            var herbs = Herb.FromHTML(Path.Combine(_datasDirname, "Herbs - Alchemy Factory Codex.html"), tables.Properties).Take(3).ToHashSet();
            tables.Recipes.UnionWith(Fertilizer.FromHTML(Path.Combine(_datasDirname, "Fertilizers - Alchemy Factory Codex.html"), tables.Properties, herbs));
            tables.Recipes.UnionWith(Recipe.FromHTML(Path.Combine(_datasDirname, "Recipes - Alchemy Factory Codex.html"), _datasDirname));
            tables.ApplyHeatIngredient();
            tables.Recipes.UnionWith(Fuel.FromHTML(Path.Combine(_datasDirname, "Fuels - Alchemy Factory Codex.html"), tables.Properties));
            foreach (var item in tables.Items)
            {
                gameDatas.AddPart(item.ToPart(prefix,_datasDirname));
            }

            foreach (var device in tables.Devices)
            {
                gameDatas.AddMachine(device.ToMachine(prefix,_datasDirname));
            }
            foreach (var recipe in tables.Recipes)
            {
                gameDatas.AddRecipe(recipe.ToRecipe(prefix));
                foreach (var ingredient in recipe.Ins)
                {
                    gameDatas.AddPart(ingredient.ToPart(prefix, _datasDirname));
                }
                foreach (var ingredient in recipe.Outs)
                {
                    gameDatas.AddPart(ingredient.ToPart(prefix, _datasDirname));
                }
            }
        }


    }
}