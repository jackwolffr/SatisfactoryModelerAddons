using HtmlAgilityPack;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Device : ItemBase
    {
        public required Dictionary<string, string> Properties { get; set; }
        public string Heat => Properties.TryGetValue("Heat Speed", out string? heat) ? heat : "";
        public static HashSet<Device> FromHTML(string dirName)
        {
            var nodes = GetDoc(dirName, "Devices - Alchemy Factory Codex.html").DocumentNode.SelectNodes("//div[@class='device']");
            return nodes.Select(node => new Device()
            {
                Name = node.SelectSingleNode("div/h3/a")?.InnerText.Trim() ?? "",
                Image = node.SelectSingleNode("div[@class='device-header']//img")?.Attributes["src"].Value ?? "",
                Properties = node.SelectNodes("div[@class='properties']/span")?.Select(node => node.InnerText.Split(":")).ToDictionary(t => t[0].Trim(), t => t[1].Trim()) ?? []
            }).ToHashSet();
        }

        public Machine ToMachine(string prefix, string datasDirname)
        {
            return new Machine(prefix)
            {
                Name = Name,
                Image = Path.Combine(datasDirname, Image),
            };
        }

    }
}