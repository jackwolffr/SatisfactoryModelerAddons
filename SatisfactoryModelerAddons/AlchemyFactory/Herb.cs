using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Herb
    {
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Nutriment { get; set; } = "";

        public static HashSet<Herb> FromHTML(string filename, Properties properties)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tbody/tr");
            return nodes.Select(node => new Herb()
            {
                Name = node.SelectSingleNode("td/div[@class='item-name-cell']/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("td/div[@class='item-name-cell']//img")?.Attributes["src"]?.Value.Trim() ?? "",
                Nutriment = node.SelectSingleNode("td[@class='total-cost-cell']/following-sibling::td/span")?.InnerText.Replace(",", "").Trim() ?? "",
            }).Where(h=>h.Nutriment!="--").ToHashSet();
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
