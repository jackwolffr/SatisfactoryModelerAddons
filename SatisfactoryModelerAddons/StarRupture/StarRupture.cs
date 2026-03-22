using log4net;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class StarRupture
    {
        public static ILog logger = LogManager.GetLogger(typeof(StarRupture));
        private readonly string _datasDirname = Path.Combine(Program.GLOBALDATADIRNAME, "StarRupture");
        private readonly string prefix = "SR";
        private Dictionary<string, string> images = [];
        public void Apply(GameDatas gameDatas)
        {
            logger.Info("************** Star Rupture **************");
            ImageMagick();
            string[] csvs = Directory.GetFiles(Path.Combine(_datasDirname, "csv"));
            foreach (var csv in csvs)
            {
                logger.Info("load csv " + csv);
                using (TextFieldParser parser = new(csv) { TextFieldType = FieldType.Delimited, Delimiters = [";"] })
                {
                    var fields = parser.ReadFields() ?? [];
                    gameDatas.AddMachine(BuildMachine(csv, fields));
                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields() ?? [];
                        logger.Info("add part " + fields[1]);
                        gameDatas.AddPart(new Part(prefix)
                        {
                            Name = fields[1],
                            Image = images[fields[1]],
                        });
                        gameDatas.AddRecipe(BuildRecipe(csv, fields));
                    }
                }
            }
            logger.Info("************** End Star Rupture **************");
        }

        private string SR_WikiggImagename(string wikifilename)
        {
            return Path.GetFileName(wikifilename)
                 .Replace("100px-", "")
                 .Replace("300px-", "")
                 .Replace("_", " ")
                 .Replace(".webp", "");
        }
        private string SR_SRToolsImagename(string wikifilename)
        {
            return Path.GetFileName(wikifilename)
                 .Replace("T_", "")
                 .Replace("_Icon.avif", "")
                 .Replace("_", " ");
        }

        private void ImageMagick()
        {
            foreach (var input in Directory.GetFiles(Path.Combine(_datasDirname, "wiki.gg")))
            {
                logger.Info("load image " + input);
                images.Add(SR_WikiggImagename(input), input);
            }
            foreach (var input in Directory.GetFiles(Path.Combine(_datasDirname, "SRToolsmachines")))
            {
                logger.Info("load image " + input);
                images.Add(SR_SRToolsImagename(input), input);
            }
            foreach (var input in Directory.GetFiles(Path.Combine(_datasDirname, "SRToolsicons")))
            {
                logger.Info("load image " + input);
                images.Add(SR_SRToolsImagename(input), input);
            }
        }

        private Machine BuildMachine(string csvfilename, string[] datas)
        {
            var name = Path.GetFileName(csvfilename).Replace(".csv", "");
            logger.Info("add machine " + name);
            var machine = new Machine(prefix)
            {
                Name = name,
                Image = images[name],
                AveragePower = "-" + datas[0]
            };

            for (int i = 2; i < datas.Length; i += 2)
            {
                machine.Costs.Add(new Ingredient(prefix)
                {
                    Name = datas[i + 1],
                    Amount = datas[i],
                });
            }
            return machine;
        }

        private Recipe BuildRecipe(string csvfilename, string[] datas)
        {
            var ins = new List<Ingredient>();
            for (var i = 3; i < datas.Length; i += 2)
            {
                ins.Add(new(prefix) { Amount = datas[i], Name = datas[i + 1], });
            }
            logger.Info("add recipe " + datas[1] + " (" + Path.GetFileName(csvfilename).Replace(".csv", "") + ")");
            return new Recipe(prefix)
            {
                Name = datas[1],
                MachineName = Path.GetFileName(csvfilename).Replace(".csv", ""),
                Time = datas[2],
                Outs = [new(prefix) { Name = datas[1], Amount = datas[0] }],
                Ins = ins
            };
        }

    }
}