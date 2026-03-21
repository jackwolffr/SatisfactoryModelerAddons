using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class RawMaterial
    {
        public string Name { get; set; } = "";
        public string Cost { get; set; } = "";

        public static HashSet<Recipe> FromHTML(string filename, Properties properties)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new RawMaterial()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Cost = ConvertCoin(node.SelectSingleNode("td[@class='total-cost-cell']")),
            }.ToRecipe(properties)).ToHashSet();
        }

        private Recipe ToRecipe(Properties properties)
        {
            return new Recipe()
            {
                Name = "Buy "+Name,
                DeviceName=properties.RawMaterialDeviceName,
                Ins=[ new Item() { 
                    Name = properties.CopperCoinName,
                    Count=Cost,
                }],
                Outs= [ new Item() {
                    Name = Name,
                    Count = "1",
                }],
                Time ="1"
            };
        }

        private static Dictionary<string, int> _coinValue=new Dictionary<string, int>() { { "copper",1 }, { "silver", 1000 }, { "gold", 100000 } };
        private static string ConvertCoin(HtmlNode? node) {
            var value = int.Parse( node?.InnerText.Replace("\u202f", "").Trim() ?? "");
            var coin = node.SelectSingleNode("span/img").Attributes["alt"]?.Value ?? "copper";
            return (value * _coinValue[coin]).ToString();
        }
    }
}
