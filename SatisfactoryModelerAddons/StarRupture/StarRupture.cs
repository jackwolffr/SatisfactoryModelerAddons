using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class StarRupture
    {
        public static ILog logger = LogManager.GetLogger(typeof(StarRupture));
        private readonly string _datasDirname = Path.Combine(Program.GLOBALDATADIRNAME, "StarRupture");
        private readonly string prefix = "SR";
        private Dictionary<string, string> images = [];
        public async Task Apply(GameDatas gameDatas)
        {
            logger.Info("************** Star Rupture **************");
            await CDNExtractor.Guess(_datasDirname);
            logger.Info("CDN is : " + CDNExtractor.CDN);

            var devices = await Device.FromHTML(_datasDirname, prefix);
            foreach (var device in devices)
            {
                gameDatas.AddMachine(new Machine(prefix)
                {
                    Name = device.Name,
                    Image = device.Image,
                    Costs = device.Costs,
                });
                foreach (var recipe in device.Recipes) {
                    gameDatas.AddRecipe(recipe);
                }
            }
            var extractors = await Extractor.FromHTML(_datasDirname, prefix);
            foreach (var extractor in extractors)
            {
                gameDatas.AddMachine(extractor.ToMachine());
                foreach (var recipe in extractor.Recipes)
                {
                    gameDatas.AddRecipe(recipe);
                }
            }
            var multiExtractors = await ExtractorMulti.FromHTML(_datasDirname, prefix);
            foreach (var multi in multiExtractors)
            {
                foreach (var multipleMachine in multi.MultipleMachines) {
                    gameDatas.AddMultipleMachine(multipleMachine);
                }
                foreach (var machine in multi.Machines)
                {
                    gameDatas.AddMachine(machine);
                }
                foreach (var recipe in multi.Recipes)
                {
                    gameDatas.AddRecipe(recipe);
                }
            }
            var items = await Item.FromHTML(_datasDirname, prefix);
            foreach (var item in items)
            {
                gameDatas.AddPart(new Part(prefix)
                {
                    Name = item.Name,
                    Image = item.Image,
                });
            }
            logger.Info("************** End Star Rupture **************");
        }
    }
}

