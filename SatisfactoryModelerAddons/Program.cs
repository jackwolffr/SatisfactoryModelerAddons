using SatisfactoryModelerAddons;
using SatisfactoryModelerAddons.AlchemyFactory;
using SatisfactoryModelerAddons.StarRupture;
using System.Text.Json;
using System.Text.Json.Nodes;

internal class Program
{
    public const string BASEDIRNAME = "D:\\workspace\\csharp\\SatisfactoryModelerAddons\\SatisfactoryModelerAddons";
    public const string OUTDIRNAME = BASEDIRNAME + "\\datasout";
    public const string IMAGEMAGICKCOMMAND = "/C magick \"{0}\" -resize {1} \"{2}\"";
    public const string GLOBALDATADIRNAME = BASEDIRNAME;

    private static void Main(string[] args)
    {
        var gameDatas = new GameDatas((JsonObject)JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(Path.Combine(GLOBALDATADIRNAME, "game_data.json"))));
        new StarRupture().Apply(gameDatas);
        new AlchemyFactory().Apply(gameDatas);
        File.WriteAllText(Path.Combine(OUTDIRNAME, "game_data/game_data.json"), JsonSerializer.Serialize(gameDatas.Root, new JsonSerializerOptions() { WriteIndented = true }));
    }

    public static void ImageMagick(string filein, string size, string fileout)
    {
        System.Diagnostics.Process.Start("CMD.exe", string.Format(IMAGEMAGICKCOMMAND, filein, size, Path.Combine(OUTDIRNAME, "images/icons", fileout)));
    }
}