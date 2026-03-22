using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class RawMaterial : ItemBase
    {
        private static ILog _logger = LogManager.GetLogger(typeof(RawMaterial));
        public string Cost { get; set; } = "";

        public static HashSet<RawMaterial> FromHTML(string dirName, Properties properties)
        {
            HtmlNodeCollection nodes = GetDoc(dirName, "Raw Materials - Alchemy Factory Codex.html").DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new RawMaterial()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"]?.Value.Trim() ?? "",
                Cost = ConvertCoin(node.SelectSingleNode("td[@class='total-cost-cell']"), properties.CoinConverter),
            }).ToHashSet();
        }
        public static HashSet<Recipe> ToRecipes(HashSet<RawMaterial> rawMaterials, Properties properties)
        {
            return rawMaterials.Select(mat => mat.ToRecipe(properties)).ToHashSet();
        }

        private Recipe ToRecipe(Properties properties)
        {
            _logger.Info($"RawMaterial {Name} = {Cost} copper");
            return new Recipe()
            {
                Name = "Buy " + Name,
                DeviceName = properties.RawMaterialDeviceName,
                Ins = [ new Item() {
                    Name = properties.CopperCoinName,
                    Image = "",
                    Count=Cost,
                }],
                Outs = [ new Item() {
                    Name = Name,
                    Image = Image,
                    Count = "1",
                }],
                Time = "1"
            };
        }

        private static string ConvertCoin(HtmlNode? node, Dictionary<string, int> coinConverter)
        {
            var value = int.Parse(node?.InnerText.Replace("\u202f", "").Trim() ?? "");
            var coin = node?.SelectSingleNode("span/img").Attributes["alt"]?.Value ?? "copper";
            return (value * coinConverter[coin]).ToString();
        }
    }
}
