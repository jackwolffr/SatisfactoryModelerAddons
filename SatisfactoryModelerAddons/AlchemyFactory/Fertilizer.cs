using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Fertilizer : ItemBase
    {
        private static ILog _logger = LogManager.GetLogger(typeof(Fertilizer));
        public string Nutriment { get; set; } = "";
        public string NutrimentSpeed { get; set; } = "";

        public static HashSet<Fertilizer> FromHTML(string dirName)
        {
            HtmlNodeCollection nodes = GetDoc(dirName, "Fertilizers - Alchemy Factory Codex.html").DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Fertilizer()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"]?.Value.Trim() ?? "",
                Nutriment = node.SelectSingleNode("td[@class='nutrient-value-cell']/span")?.InnerText.Replace("\u202f", "").Trim() ?? "",
                NutrimentSpeed = node.SelectSingleNode("td[@class='nutrient-value-cell']/following-sibling::td/span")?.InnerText.Replace(",", "").Trim() ?? "",
            }).ToHashSet();
        }

        public static HashSet<Recipe> ToRecipes(HashSet<Fertilizer> fertilizers, Properties properties, HashSet<Herb> herbs)
        {
            return properties.Nursery.SelectMany(n => fertilizers, (n, f) => f.ToRecipe(properties, herbs.Where(h => n.Contains(h.Name)))).ToHashSet();
        }
        public Recipe ToRecipe(Properties properties, IEnumerable<Herb> herbs)
        {
            var nutriment = double.Parse(Nutriment, CultureInfo.InvariantCulture);
            var nutrimentSpeed = double.Parse(NutrimentSpeed, CultureInfo.InvariantCulture);
            _logger.Info($"{Name} => " + string.Join(" and ", herbs.Select(h => nutriment + "/(" + h.Nutriment + " * " + herbs.Count() + ") " + h.Name)));
            return new Recipe()
            {
                Name = string.Join(" and ", herbs.Select(h => h.Name)) + " " + Name,
                DeviceName = properties.NurseryDeviceName,
                Ins = [new Item() {
                    Name = Name,
                    Image = Image,
                    Count="1"
                }],
                Outs = herbs.Select(herb => new Item()
                {
                    Name = herb.Name,
                    Image = herb.Image,
                    Count = (nutriment / (herbs.Count() * double.Parse(herb.Nutriment, CultureInfo.InvariantCulture))).ToString(CultureInfo.InvariantCulture),
                }).ToList(),
                Time = (nutriment / nutrimentSpeed).ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}
