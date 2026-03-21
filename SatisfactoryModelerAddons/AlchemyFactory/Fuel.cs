using HtmlAgilityPack;
using System.Text.Json.Nodes;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Fuel
    {
        public required string Name { get; set; }
        public required string Heat { get; set; }

        public static HashSet<Recipe> FromHTML(string filename, Properties properties)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Fuel()
            {
                Name = node.SelectNodes("td/div[@class='item-name-cell']/a").FirstOrDefault()?.InnerText.Trim() ?? "",
                Heat = node.SelectNodes("td[@class='heat-value-cell']").FirstOrDefault()?.InnerText.Replace("\u202f", "").Trim() ?? "",
            }.ToRecipe(properties)).ToHashSet();
        }

        public Recipe ToRecipe(Properties properties)
        {
            return new Recipe()
            {
                Name = "Fuel " + Name,
                DeviceName = properties.FuelDeviceName,
                Ins = [new Item() {
                    Count = "1",
                    Name = Name,
                }],
                Outs = [new Item() {
                    Count = Heat,
                    Name = properties.FuelItemName,
                }],
                Time = "1"
            };
        }
    }
}