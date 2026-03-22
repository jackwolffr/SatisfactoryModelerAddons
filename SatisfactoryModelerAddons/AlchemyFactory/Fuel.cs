using HtmlAgilityPack;
using System.Text.Json.Nodes;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Fuel : ItemBase
    {
        public required string Heat { get; set; }

        public static HashSet<Fuel> FromHTML(string dirName)
        {
            HtmlNodeCollection nodes = GetDoc(dirName, "Fuels - Alchemy Factory Codex.html").DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Fuel()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"].Value.Trim() ?? "",
                Heat = node.SelectSingleNode("td[@class='heat-value-cell']")?.InnerText.Replace("\u202f", "").Trim() ?? "",
            }).ToHashSet();
        }

        public static HashSet<Recipe> ToRecipes(HashSet<Fuel> fuels, Properties properties)
        {
            return fuels.Select(f => f.ToRecipe(properties)).ToHashSet();
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
                    Image = Image
                }],
                Outs = [new Item() {
                    Count = Heat,
                    Name = properties.FuelItemName,
                    Image = ""
                }],
                Time = "1"
            };
        }
    }
}