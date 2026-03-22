using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class ItemBase
    {
        public required string Name { get; set; }
        public string Image { get; set; } = "";

        public static HtmlDocument GetDoc(string dirName, string fileName)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(File.ReadAllText(Path.Combine(dirName,fileName)));
            return doc;
        }
        public Part ToPart(string prefix, string datasDirname)
        {
            return new Part(prefix)
            {
                Name = Name,
                Image = Path.Combine(datasDirname, Image)
            };
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
