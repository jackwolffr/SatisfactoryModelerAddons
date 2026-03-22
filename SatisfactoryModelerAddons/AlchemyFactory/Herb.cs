using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Herb : ItemBase
    {
        public string Nutriment { get; set; } = "";

        public static HashSet<Herb> FromHTML(string dirName)
        {
            HtmlNodeCollection nodes = GetDoc(dirName, "Herbs - Alchemy Factory Codex.html").DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Herb()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"]?.Value.Trim() ?? "",
                Nutriment = node.SelectSingleNode("td[@class='total-cost-cell']/following-sibling::td/span")?.InnerText.Replace(",", "").Trim() ?? "",
            }).ToHashSet();
        }
    }
}
