using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.AlchemyFactory
{
    public class Tables
    {
        public HashSet<Device> Devices { get; set; } = [];
        public Properties Properties { get; set; } = new Properties();
        public HashSet<Recipe> Recipes { get; set; } = [];
        public HashSet<Item> Items { get; set; } = [];

        public void ApplyHeatIngredient()
        {
            foreach (var recipe in Recipes)
            {
                var device = Devices.FirstOrDefault(d => d.Name == recipe.DeviceName);
                if (device != null && device.Name != recipe.Name && device.Heat != "")
                {
                    recipe.Ins.Add(new Item()
                    {
                        Name = Properties.FuelItemName,
                        Count = (double.Parse(device.Heat, CultureInfo.InvariantCulture) * double.Parse(recipe.Time, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
        }
    }
    public class Properties
    {
        public string FuelDeviceName { get; set; } = string.Empty;
        public string FuelItemName { get; set; } = string.Empty;
        public string RawMaterialDeviceName { get; set; } = string.Empty;
        public string CopperCoinName { get; set; } = string.Empty;
        public string NurseryDeviceName { get; set; } = string.Empty;
    }
}
