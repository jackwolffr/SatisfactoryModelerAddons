using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;


namespace SatisfactoryModelerAddons.StarRupture
{
    public class Device
    {
        private string _prefix;
        public required string Name { get; set; }
        public string Image { get; set; } = "";
        public Device(string prefix)
        {
            _prefix = prefix;
        }

        public required List<Recipe> Recipes { get; set; }
        public required List<Ingredient> Costs { get; set; }
        public async static Task<IEnumerable<Device>> FromHTML(string dirName, string prefix)
        {
            var v1 = string.Empty;
            var v2 = " v.2";
            return await Task.WhenAll(
            [
                GetDevice("https://starrupture.tools/buildings/assembler",v1, dirName,  prefix),
                GetDevice("https://starrupture.tools/buildings/factory",v1, dirName,  prefix),
                GetDevice("https://starrupture.tools/buildings/factory-tier2",v2, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/crafter",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/crafter-tier2",v2, dirName,  prefix),
                GetDevice("https://starrupture.tools/buildings/military-assembler",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/synthetizer",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/synthetizer-tier2",v2, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/furnace",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/furnace-tier2",v2, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/hammer",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/forge",v1, dirName, prefix),
                GetDevice("https://starrupture.tools/buildings/refinery",v1, dirName, prefix), // les 2 sont inversés sur le site Refinery <-> pressurizer 
                GetDevice("https://starrupture.tools/buildings/pressurizer",v1, dirName, prefix),
            ]);
        }
        private async static Task<Device> GetDevice(string url, string version, string dirName, string prefix)
        {
            var nodes = (await Resource.GetDocAsync(url, dirName)).DocumentNode.SelectNodes("//script");
            JNode building = nodes
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
                .SearchObject("building", System.Text.Json.JsonValueKind.Object);
            var name = building["name"].Str() ?? string.Empty;
            return new Device(prefix)
            {
                Name = name,
                Image = await Resource.DownloadAsync(CDNExtractor.GetFullUrl(building["icon"].Str() ?? string.Empty), dirName, "R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="),
                Recipes = building["recipes"].EnumerateArray().Select(recipe => new Recipe(prefix)
                {
                    Name = (recipe["output"]["displayName"].Str() ?? string.Empty) + version,
                    MachineName = name,
                    Time = recipe["duration"].Int()?.ToString() ?? string.Empty,
                    Outs = [new Ingredient(prefix) {
                        Name = recipe["output"]["item"].Walk(building,"building")["name"].Str() ?? string.Empty,
                        Amount = recipe["output"]["quantity"].Int()?.ToString() ?? string.Empty
                    }],
                    Ins = recipe["inputs"].EnumerateArray().Select(input => new Ingredient(prefix)
                    {
                        Name = input["item"].Walk(building, "building")["name"].Str() ?? string.Empty,
                        Amount = input["quantity"].Int()?.ToString() ?? string.Empty
                    }).ToList()
                }).ToList(),
                Costs = building["requirements"].EnumerateArray().Select(req =>
                    new Ingredient(prefix)
                    {
                        Amount = req["quantity"].Int()?.ToString() ?? string.Empty,
                        Name = req["item"].Walk(building, "building")["name"].Str() ?? string.Empty
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
