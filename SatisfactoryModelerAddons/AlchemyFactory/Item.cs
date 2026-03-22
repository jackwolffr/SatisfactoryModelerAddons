
using HtmlAgilityPack;
using System.Globalization;

namespace SatisfactoryModelerAddons.AlchemyFactory
{

    public class Item : ItemBase
    {
        public string Count { get; set; } = "1";
        public string Percentage { get; set; } = "100";

        public static Item FromIngredientNode(HtmlNode node)
        {
            var parts = node.SelectNodes("div/span")?.FirstOrDefault()?.InnerText.Trim().Split('(')[0].Split(' ', 2) ?? ["", ""];
            return new Item()
            {
                Name = parts[1].Trim(),
                Count = parts[0].Replace("x", "").Trim(),
                Image = node.SelectSingleNode("div//img")?.Attributes["src"]?.Value ?? "",
                Percentage = node.SelectSingleNode("div//span[@class='percentage']")?.InnerText.Replace("(", "").Replace("%)", "").Trim() ?? "100",
            };
        } 

        public Ingredient ToIngredient(string prefix) {
            return new Ingredient(prefix)
            {
                Name = Name,
                Amount = (double.Parse(Count,CultureInfo.InvariantCulture) * double.Parse(Percentage, CultureInfo.InvariantCulture) / 100).ToString(CultureInfo.InvariantCulture),
            };
        }

    }
}