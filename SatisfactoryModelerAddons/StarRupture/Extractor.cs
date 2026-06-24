using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class Extractor
    {
        private string _prefix;

        public Extractor(string prefix)
        {
            _prefix = prefix;
        }
        public required string Name { get; set; }

        public string Image { get; set; } = "";

        public required List<Recipe> Recipes { get; set; }
        public required List<Ingredient> Costs { get; set; }

        public async static Task<IEnumerable<Extractor>> FromHTML(string dirName, string prefix)
        {
            return await Task.WhenAll(
            [
                GetExtractor("https://starrupture.tools/buildings/gas-extractor", dirName,  prefix),
                GetExtractor("https://starrupture.tools/buildings/laser-drill", dirName,  prefix),
                GetExtractor("https://starrupture.tools/buildings/liquid-extractor", dirName,  prefix),
                GetExtractor("https://starrupture.tools/buildings/acid-extractor", dirName,  prefix),
            ]);
        }
        private async static Task<Extractor> GetExtractor(string url, string dirName, string prefix)
        {
            var nodes = (await Resource.GetDocAsync(url, dirName)).DocumentNode.SelectNodes("//script");
            JNode building = nodes
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
                .SearchObject("building", System.Text.Json.JsonValueKind.Object);
            var name = building["name"].Str() ?? string.Empty;
            return new Extractor(prefix)
            {
                Name = name,
                Image = await Resource.DownloadAsync(CDNExtractor.GetFullUrl(building["icon"].Str() ?? string.Empty), dirName, "R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="),
                Recipes = building["recipes"].EnumerateArray().Select(recipe => new Recipe(prefix)
                {
                    Name = recipe["output"]["item"].Walk(building,"building")["name"].Str() ?? string.Empty,
                    MachineName = name,
                    Time = recipe["duration"].Int()?.ToString() ?? string.Empty,
                    Outs = [new Ingredient(prefix) {
                        Name = recipe["output"]["item"].Walk(building,"building")["name"].Str() ?? string.Empty,
                        Amount = recipe["output"]["quantity"].Int()?.ToString() ?? string.Empty
                    }],
                    Ins = []
                }).ToList(),
                Costs = building["requirements"].EnumerateArray().Select(req =>
                    new Ingredient(prefix)
                    {
                        Amount = req["quantity"].Int()?.ToString() ?? string.Empty,
                        Name = req["item"]["name"].Str() ?? string.Empty
                    }).ToList()
            };
        }

        public Machine ToMachine()
        {
            return new Machine(_prefix)
            {
                Image = Image,
                Name = Name,
                Costs = Costs
            };
        }
    }
}
