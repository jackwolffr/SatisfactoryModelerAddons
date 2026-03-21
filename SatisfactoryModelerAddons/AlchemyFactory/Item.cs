
using HtmlAgilityPack;
using System.Globalization;

namespace SatisfactoryModelerAddons.AlchemyFactory
{

    public class Item
    {
        public required string Name { get; set; }
        public string Count { get; set; } = "1";
        public string Image { get; set; } = "";
        public string Percentage { get; set; } = "100";

        public static Item FromIngredientNode(HtmlNode node, string dataDirName)
        {
            var parts = node.SelectNodes("div/span")?.FirstOrDefault()?.InnerText.Trim().Split('(')[0].Split(' ', 2) ?? ["", ""];
            return new Item()
            {
                Name = parts[1].Trim(),
                Count = parts[0].Replace("x", "").Trim(),
                Image = Path.Combine(dataDirName, node.SelectNodes("div//img")?.FirstOrDefault()?.Attributes["src"]?.Value ?? ""),
                Percentage = node.SelectNodes("div//span[@class='percentage']")?.FirstOrDefault()?.InnerText.Replace("(", "").Replace("%)", "").Trim() ?? "100",
            };
        } 

        public Part ToPart(string prefix, string datasDirname)
        {
            return new Part(prefix)
            {
                Name = Name,
                Image = Path.Combine(datasDirname, Image)
            };
        }

        public Ingredient ToIngredient(string prefix) {
            return new Ingredient(prefix)
            {
                Name = Name,
                Amount = (double.Parse(Count,CultureInfo.InvariantCulture) * double.Parse(Percentage, CultureInfo.InvariantCulture) / 100).ToString(CultureInfo.InvariantCulture),
            };
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Recipe recipe && Name == recipe.Name;
        }
    }
}