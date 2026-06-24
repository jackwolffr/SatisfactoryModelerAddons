using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SatisfactoryModelerAddons
{
    public class MultipleMachine
    {
        private MultipleMachine()
        {
        }

        public MultipleMachine(string prefix)
        {
            Prefix = prefix;
        }
        public string FinalName => Prefix + " " + Name;
        public string Name { get; set; } = "";
        public bool ShowPpm { get; set; } = true;
        public bool AutoRound { get; set; } = false;
        public string DefaultMax { get; set; } = "60";
        public List<MultipleMachineDef> Machines { get; set; } = [];
        public List<MultipleMachineCapacity> Capacities { get; set; } = [];
        public string Prefix { get; } = "";

        public JsonObject Build()
        {
            return new JsonObject
            {
                { "Name", FinalName },
                { "ShowPpm", ShowPpm},
                { "AutoRound", AutoRound },
                { "DefaultMax", DefaultMax },
                { "Machines", BuildMachines() },
                { "Capacities", BuildCapacities()}
            };
        }

        public MultipleMachine SatisfactoryCapacities()
        {
            Capacities.Add(MultipleMachineCapacity.Impure);
            Capacities.Add(MultipleMachineCapacity.Normal);
            Capacities.Add(MultipleMachineCapacity.Pure);
            return this;
        }

        private JsonArray BuildMachines()
        {
            var array = new JsonArray();
            foreach (var item in Machines)
            {
                array.Add(item.Build());
            }
            return array;
        }

        private JsonArray BuildCapacities()
        {
            var array = new JsonArray();
            foreach (var cap in Capacities)
            {
                array.Add(cap.Build());
            }
            return array;
        }
    }
}
