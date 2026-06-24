using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons
{
    public class MultipleMachineCapacity
    {
        public static readonly int impureColor = 13775920;
        public static readonly int pureColor = 8433977;

        public static readonly MultipleMachineCapacity Impure = new MultipleMachineCapacity()
        {
            Name = "Impure",
            PartsRatio = "1/2",
            Color = impureColor
        };
        public static readonly MultipleMachineCapacity Normal = new MultipleMachineCapacity()
        {
            Name = "Normal",
            Default = true
        };
        public static readonly MultipleMachineCapacity Pure = new MultipleMachineCapacity()
        {
            Name = "Pure",
            PartsRatio = "2",
            Color = pureColor
        };


        public MultipleMachineCapacity()
        {

        }
        public string Name { get; set; } = "";
        public string PartsRatio { get; set; } = "";
        public int? Color { get; set; }
        public bool? Default { get; set; }
        public JsonObject Build()
        {
            var ret = new JsonObject
            {
                { "Name", Name },
                { "PartsRatio", PartsRatio}
            };
            if (Color != null)
            {
                ret.Add("Color", Color);
            }
            if (Default != null)
            {
                ret.Add("Default", Default);
            }
            return ret;
        }

    }
}
