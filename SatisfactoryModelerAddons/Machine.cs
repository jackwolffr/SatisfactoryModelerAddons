using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class Machine
    {
        private Machine()
        {
        }

        public Machine(string prefix)
        {
            Prefix = prefix;
        }

        public string FinalName => Prefix + " " + Name;
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Tier { get; set; } = "0-2";
        public string AveragePower { get; set; } = "0";
        public string OverclockPowerExponent { get; set; } = "1000000/1000000";
        public string MaxProductionShards { get; set; } = "0";
        public string ProductionShardMultiplier { get; set; } = "1";
        public string ProductionShardPowerExponent { get; set; } = "2";
        public List<Ingredient> Costs { get; set; } = [];
        public string Prefix { get; } = "";

        public JsonObject Build()
        {
            return new JsonObject
            {
                { "Name", FinalName },
                { "Tier", Tier},
                { "AveragePower", AveragePower },
                { "OverclockPowerExponent", OverclockPowerExponent },
                { "MaxProductionShards", MaxProductionShards },
                { "ProductionShardMultiplier",ProductionShardMultiplier },
                { "ProductionShardPowerExponent", ProductionShardPowerExponent },
                { "Cost", BuildCosts()}
            };
        }

        private JsonArray BuildCosts()
        {
            var array = new JsonArray();
            foreach (var item in Costs) {
                array.Add(item.Build());
            }
            return array;
        }
    }
}
