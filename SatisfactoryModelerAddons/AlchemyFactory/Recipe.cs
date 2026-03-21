
using HtmlAgilityPack;
using System.Diagnostics.CodeAnalysis;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Recipe
    {
        public required string Name { get; set; }
        public required string DeviceName { get; set; }
        public required string Time { get; set; }
        public required List<Item> Ins { get; set; }
        public required List<Item> Outs { get; set; }

        public SatisfactoryModelerAddons.Recipe ToRecipe(string prefix)
        {
            return new SatisfactoryModelerAddons.Recipe(prefix)
            {
                Name = Name,
                MachineName = DeviceName,
                Time = Time,
                Ins = Ins.Select(item => item.ToIngredient(prefix)).ToList(),
                Outs = Outs.Select(item => item.ToIngredient(prefix)).ToList()
            };
        }

        public static HashSet<Recipe> FromHTML(string filename, string dataDirName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='recipe']");
            return nodes.Select(node => new Recipe()
            {
                Name = node.SelectNodes("h3/a").FirstOrDefault()?.InnerText.Replace("Recipe:", "").Trim() ?? "",
                DeviceName = node.SelectNodes("div[@class='recipe-details']//a").FirstOrDefault()?.InnerText.Trim() ?? "",
                Time = node.SelectNodes("div[@class='recipe-details']/span").FirstOrDefault()?.InnerText.Replace("Time:", "").Replace("s", "").Trim() ?? "",
                Ins = node.SelectNodes("div/div[@class='ingredients']/ul/li").Select(node => Item.FromIngredientNode(node, dataDirName)).ToList(),
                Outs = node.SelectNodes("div/div[@class='outputs']/ul/li").Select(node => Item.FromIngredientNode(node, dataDirName)).ToList(),
            }).ToHashSet();
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