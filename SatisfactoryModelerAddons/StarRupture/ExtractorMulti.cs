using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class ExtractorMulti
    {
        private string _prefix;

        public ExtractorMulti(string prefix)
        {
            _prefix = prefix;
        }

        public required List<MultipleMachine> MultipleMachines { get; set; }
        public required List<Machine> Machines { get; set; }
        public required List<Recipe> Recipes { get; set; }

        public async static Task<IEnumerable<ExtractorMulti>> FromHTML(string dirName, string prefix)
        {
            // la V2 est totalement foireuse 23/06/2026
            return await Task.WhenAll(
            [
                GetMultiExtractor("https://starrupture.tools/buildings/mechanical-drill","https://starrupture.tools/buildings/mechanical-drill-tier2", dirName,  prefix),
            ]);
        }
        private async static Task<ExtractorMulti> GetMultiExtractor(string urlv1, string urlv2, string dirName, string prefix)
        {
            var nodesv1 = (await Resource.GetDocAsync(urlv1, dirName)).DocumentNode.SelectNodes("//script");
            JNode buildingv1 = nodesv1
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
                .SearchObject("building", System.Text.Json.JsonValueKind.Object);
            var nodesv2 = (await Resource.GetDocAsync(urlv2, dirName)).DocumentNode.SelectNodes("//script");
            JNode buildingv2 = nodesv2
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
                .SearchObject("building", System.Text.Json.JsonValueKind.Object);
            //buildingv1.Debug(Path.Combine(dirName, "buildingv1.txt"));
            //buildingv2.Debug(Path.Combine(dirName, "buildingv2.txt"));
            var namev1 = buildingv1["name"].Str() ?? string.Empty;
            var namev2 = buildingv2["name"].Str() ?? string.Empty;

            var table = buildingv1["recipes"].EnumerateArray().ToDictionary(recipe => recipe["output"]["displayName"].Str() ?? string.Empty, recipe => recipe["output"]["quantity"].Int() ?? 0);
            // 
            var recipes = new Dictionary<string, Recipe>();
            var puritiesv1 = new Dictionary<string, MultipleMachineCapacity>() {
            { "Impure", new MultipleMachineCapacity()
                {
                    Color=MultipleMachineCapacity.impureColor,
                    Name="Impure"
                }
            },
            { "Normal", new MultipleMachineCapacity()
                {
                    Name="Normal",
                    Default=true
                }
            },
            { "Pure", new MultipleMachineCapacity()
                {
                    Color=MultipleMachineCapacity.pureColor,
                    Name="Pure"
                }
            },
            };
            var nameParser = new Regex(@"([\w ]+) \(([\w]+)\)");
            foreach (var recipe in buildingv1["recipes"].EnumerateArray())
            {
                var matches = nameParser.Match(recipe["output"]["displayName"].Str() ?? string.Empty);
                recipes[matches.Groups[1].Value] = new Recipe(prefix)
                {
                    Ins = [],
                    MachineName = namev1,
                    Name = matches.Groups[1].Value,
                    Time = (recipe["duration"].Int() ?? 0).ToString(),
                    Outs = [
                        new(prefix){
                            Amount="1",
                            Name= matches.Groups[1].Value,
                        }],
                };
                recipes[matches.Groups[1].Value+ " v.2"] = new Recipe(prefix)
                {
                    Ins = [],
                    MachineName = namev2,
                    Name = matches.Groups[1].Value + " v.2",
                    Time = (recipe["duration"].Int() ?? 0).ToString(),
                    Outs = [
                        new(prefix){
                            Amount="1",
                            Name= matches.Groups[1].Value,
                        }],
                };
                puritiesv1[matches.Groups[2].Value].PartsRatio = (recipe["output"]["quantity"].Int() ?? 0).ToString();
            }
            var puritiesv2 = new Dictionary<string, MultipleMachineCapacity>() {
            { "Impure", new MultipleMachineCapacity()
                {
                    Color=MultipleMachineCapacity.impureColor,
                    Name="Impure",
                    PartsRatio="6"
                }
            },
            { "Normal", new MultipleMachineCapacity()
                {
                    Name="Normal",
                    Default=true,
                    PartsRatio="10"
                }
            },
            { "Pure", new MultipleMachineCapacity()
                {
                    Color=MultipleMachineCapacity.pureColor,
                    Name="Pure",
                    PartsRatio="16"
                }
            },
            };



            return new ExtractorMulti(prefix)
            {
                Recipes = recipes.Values.ToList(),
                MultipleMachines = [
                    new MultipleMachine(prefix)
                    {
                        AutoRound = false,
                        DefaultMax = "30",
                        Name = namev1,
                        ShowPpm = true,
                        Machines = [],
                        Capacities=puritiesv1.Values.ToList(),
                    },
                    new MultipleMachine(prefix)
                    {
                        AutoRound = false,
                        DefaultMax = "30",
                        Name = namev2,
                        ShowPpm = true,
                        Machines = [],
                        Capacities=puritiesv2.Values.ToList(),
                    },
                    ],
                Machines = [
                    new(prefix)
                    {
                        Name = namev1,
                        Image = await Resource.DownloadAsync(CDNExtractor.GetFullUrl(buildingv1["icon"].Str() ?? string.Empty), dirName, "R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="),
                        Costs = buildingv1["requirements"].EnumerateArray().Select(req => new Ingredient(prefix)
                            {
                                Amount = req["quantity"].Int()?.ToString() ?? string.Empty,
                                Name = req["item"]["name"].Str() ?? string.Empty
                            }).ToList(),
                    },
                    new(prefix)
                    {
                        Name = namev2,
                        Image = await Resource.DownloadAsync(CDNExtractor.GetFullUrl(buildingv2["icon"].Str() ?? string.Empty), dirName, "R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="),
                        Costs = buildingv2["requirements"].EnumerateArray().Select(req => new Ingredient(prefix)
                            {
                                Amount = req["quantity"].Int()?.ToString() ?? string.Empty,
                                Name = req["item"]["name"].Str() ?? string.Empty
                            }).ToList(),
                    },
                ]
            };
        }

        private static string RectifiedAmount(string name)
        {
            return name switch
            {
                var s when s.Contains("Impure") => "6",
                var s when s.Contains("Normal") => "10",
                var s when s.Contains("Pure") => "16",
                _ => "0"
            };
        }
    }
}
