using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class Item
    {
        public required string Name { get; set; }
        public required string Image { get; set; }

        public async static Task<IEnumerable<Item>> FromHTML(string dirName, string prefix)
        {
            var nodes = (await Resource.GetDocAsync("https://starrupture.tools/items", dirName)).DocumentNode.SelectNodes("//script");
            JNode items = nodes
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
                .SearchObject("items", System.Text.Json.JsonValueKind.Array);
            return await Task.WhenAll(items.EnumerateArray().Select(async item => new Item()
            {
                Name = item["name"].Str() ?? string.Empty,
                Image = await Resource.DownloadAsync(CDNExtractor.GetFullUrl(item["icon"].Str() ?? string.Empty), dirName, "R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="),
            }));
        }
    }
}
