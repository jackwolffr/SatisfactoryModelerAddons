using ImageMagick;
using log4net;
using log4net.Config;
using SatisfactoryModelerAddons;
using SatisfactoryModelerAddons.AlchemyFactory;
using SatisfactoryModelerAddons.StarRupture;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

public class Program
{
    public static ILog logger = LogManager.GetLogger(typeof(Program));
    public static readonly string BASEDIRNAME = Environment.CurrentDirectory;
    public static readonly string OUTDIRNAME = Path.Combine(BASEDIRNAME , "datasout");
    public static readonly string GAMEDATASDIRNAME = Path.Combine(OUTDIRNAME , "game_data");
    public static readonly string IMAGESDIRNAME = Path.Combine(OUTDIRNAME , "images","icons");
    public static readonly string IMAGEMAGICKCOMMAND = "/C magick \"{0}\" -resize {1} \"{2}\"";
    public static readonly string GLOBALDATADIRNAME = Path.Combine(BASEDIRNAME,"datas");

    private async static Task Main(string[] args)
    {
        logger.Info("build directory "+ GAMEDATASDIRNAME);
        Directory.CreateDirectory(GAMEDATASDIRNAME);
        logger.Info("build directory " + IMAGESDIRNAME);
        try
        {
            Directory.Delete(IMAGESDIRNAME, true);
        }
        catch (DirectoryNotFoundException)
        {
            //
        }
        Directory.CreateDirectory(IMAGESDIRNAME);
        logger.Info("load game_data.json");
        var gameDatas = new GameDatas((JsonObject?)JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(Path.Combine(GLOBALDATADIRNAME, "game_data.json"))));
        await new StarRupture().Apply(gameDatas);
        //new AlchemyFactory().Apply(gameDatas);
        logger.Info("write game_data.json");
        File.WriteAllText(Path.Combine(GAMEDATASDIRNAME, "game_data.json"), JsonSerializer.Serialize(gameDatas.Root, new JsonSerializerOptions() { WriteIndented = true }));
    }

    public static void ImageMagick(string filein, IMagickGeometry geometry, string fileout)
    {
        logger.Info("load image " + filein);
        using var image = new MagickImage(filein);
        image.Resize(geometry);
        logger.Info("write image " + fileout);
        image.Write(Path.Combine(IMAGESDIRNAME, fileout));
    }
}