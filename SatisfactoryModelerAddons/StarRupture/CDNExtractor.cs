using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SatisfactoryModelerAddons.StarRupture
{
    public class CDNExtractor
    {
        public static string CDN { get; set; } = "https://cdn.pixel6.tools/starrupture/images/";
        public static string ImageVersion { get; set; }

        // la donnée n'est pas directement présente dans les fichiers React, elle est planqué dans un chunk et dans une fonciton anonyme, et meme React est obligé d'appeler la meme méthode pour toutes les urls
        // donc au lieu de se risquer à décrypter les chunk, je passe par og:image qui pointe aussi sur le CDN
        // et en comparant avec une url relative (celle du titanium), j'en déduis le CDN
        // c'est du bricolage, mais j'ai pas mieux
        public async static Task Guess(string dirName)
        {
            var doc = await Resource.GetDocAsync("https://starrupture.tools/items/titanium-ore", dirName);
            var ogImageUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", string.Empty);
            if (ogImageUrl == null)
            {
                return;
            }
            var ogImage = new Uri(ogImageUrl);
            ImageVersion = System.Web.HttpUtility.ParseQueryString(ogImage.Query)["v"] ?? string.Empty;

            JNode item = doc.DocumentNode.SelectNodes("//script")
                .Select(n => n.InnerText)
                .CleanJS()
                .ReactFlightBloc()
                .ReactFlightToJson()
            .SearchObject("item", System.Text.Json.JsonValueKind.Object);
            CDN = ogImage.AbsoluteUri
                .Replace(ogImage.Query, "")
                .Replace(item["icon"].Str() ?? string.Empty, "");
        }

        public static string GetFullUrl(string url)
        {
            return CDN + url + "?v=" + ImageVersion;
        }

    }
}
