using Microsoft.VisualBasic.FileIO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class StarRupture
    {
        private readonly string _datasDirname = Program.GLOBALDATADIRNAME + "\\StarRupture\\datas";
        private readonly string prefix = "SR";
        private Dictionary<string, string> images = [];
        public void Apply(GameDatas gameDatas)
        {
            ImageMagick();
            string[] csvs = Directory.GetFiles(Path.Combine(_datasDirname, "csv"));
            foreach (var csv in csvs)
            {
                using (TextFieldParser parser = new(csv) {TextFieldType = FieldType.Delimited, Delimiters= [";"] })
                {
                    var fields = parser.ReadFields() ?? [];
                    gameDatas.AddMachine(BuildMachine(csv, fields));
                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields() ?? [];
                        gameDatas.AddPart(new Part(prefix)
                        {
                            Name = fields[1],
                            Image = images[fields[1]],
                        });
                        gameDatas.AddRecipe(BuildRecipe(csv, fields));
                    }
                }
            }
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
                images.Add(SR_WikiggImagename(input), input);
            }
            foreach (var input in Directory.GetFiles(Path.Combine(_datasDirname, "SRToolsmachines")))
            {
                images.Add(SR_SRToolsImagename(input), input);
            }
            foreach (var input in Directory.GetFiles(Path.Combine(_datasDirname, "SRToolsicons")))
            {
                images.Add(SR_SRToolsImagename(input), input);
            }
        }

        private Machine BuildMachine(string csvfilename, string[] datas)
        {
            var name = Path.GetFileName(csvfilename).Replace(".csv", "");
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
                ins.Add(new Ingredient(prefix)
                {
                    Amount = datas[i],
                    Name = datas[i + 1],
                });
            }
            return new Recipe(prefix)
            {
                Name = datas[1],
                MachineName = Path.GetFileName(csvfilename).Replace(".csv", ""),
                Time = datas[2],
                Outs = [new(prefix) { Name =datas[1], Amount = datas[0] }],
                Ins = ins
            };
        }

    }
}