using HtmlAgilityPack;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Device
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
        public required string Heat { get; set; }

        public Machine ToMachine(string prefix, string datasDirname)
        {
            return new Machine(prefix)
            {
                Name = Name,
                Image = Path.Combine(datasDirname, Image),
            };
        }

        public static HashSet<Device> FromHTML(string filename, string dataDirName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filename));
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='device']");
            return nodes.Select(node => new Device()
            {
                Name = node.SelectNodes("div/h3/a")?.First().InnerText.Trim() ?? "",
                Image = Path.Combine(dataDirName, node.SelectNodes("div[@class='device-header']//img")?.First().Attributes["src"].Value ?? ""),
                Heat = node.SelectNodes("div[@class='properties']/span")?.Where(node => node.InnerText.Contains("Heat Speed")).First().InnerText.Replace("Heat Speed:", "").Trim() ?? "",
            }).ToHashSet();
        }

        public override bool Equals(object? obj)
        {
            return obj is Device device && Name == device.Name;
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