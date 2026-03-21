using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Fertilizer
    {
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Nutriment { get; set; } = "";
        public string NutrimentSpeed { get; set; } = "";

        public static HashSet<Recipe> FromHTML(string filename, Properties properties, HashSet<Herb> herbs)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Fertilizer()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"]?.Value.Trim() ?? "",
                Nutriment = node.SelectSingleNode("td[@class='nutrient-value-cell']/span")?.InnerText.Replace("\u202f", "").Trim() ?? "",
                NutrimentSpeed = node.SelectSingleNode("td[@class='nutrient-value-cell']/following-sibling::td/span")?.InnerText.Replace(",", "").Trim() ?? "",
            }).SelectMany(f => herbs, (f, h) => f.ToRecipe(properties, h)).ToHashSet();
        }

        private Recipe ToRecipe(Properties properties, Herb herb)
        {
            var nutriment = double.Parse(Nutriment, CultureInfo.InvariantCulture);
            var nutrimentSpeed = double.Parse(NutrimentSpeed, CultureInfo.InvariantCulture);
            var herbNutriment = double.Parse(herb.Nutriment, CultureInfo.InvariantCulture);

            return new Recipe()
            {
                Name = herb.Name + " " + Name,
                DeviceName = properties.NurseryDeviceName,
                Ins = [new Item() {
                    Name = Name,
                    Image = Image,
                    Count="1"
                }],
                Outs = [new Item() {
                    Name = herb.Name,
                    Image = herb.Image,
                    Count=(nutriment / herbNutriment).ToString(CultureInfo.InvariantCulture),
                }],
                Time = (nutriment / nutrimentSpeed).ToString(CultureInfo.InvariantCulture),
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is Recipe recipe && Name == recipe.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
