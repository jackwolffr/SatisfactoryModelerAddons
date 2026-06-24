using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SatisfactoryModelerAddons.StarRupture
{
    public static class StringExtension
    {
        public static IEnumerable<string> CleanJS(this IEnumerable<string> stream)
        {
            foreach (var text in stream)
            {
                if (text.StartsWith("self.__next_f.push([1,\""))
                {
                    var jsontext = text.Substring(23).Replace("\\\"", "\"");
                    yield return jsontext.Substring(0, jsontext.Length - 3);
                }
            }
        }
        public static IEnumerable<string> ReactFlightBloc(this IEnumerable<string> stream)
        {
            var sb = new StringBuilder();
            foreach (var text in stream)
            {
                var parts = text.Split("\\n");
                sb.Append(parts[0]);
                for (var i = 1; i < parts.Length; i++)
                {
                    yield return sb.ToString();
                    sb.Clear();
                    sb.Append(parts[i]);
                }
            }
            yield return sb.ToString();
        }
        public static IEnumerable<JsonDocument> ReactFlightToJson(this IEnumerable<string> stream)
        {
            foreach (var text in stream)
            {
                var colonpos = text.IndexOf(':'); // le bloc commence par un numero/lettre/rien puis ":" puis parfois "I", ex 10:I ou a: ou :I
                if (colonpos >= 0 && colonpos < 5 && "{[".Contains(text[colonpos + 1])) // vu que c'est un json, il y a plein de :, donc on en repere un dans les 5 premiers char, et on vire tout le protocole ReactFlight en ne prenant que le json direct qui commence par { ou [
                {
                    yield return JsonDocument.Parse(text.Substring(colonpos + 1));
                }
            }
        }
        public static JsonElement? SearchObject(this IEnumerable<JsonDocument> stream, string name, JsonValueKind kind)
        {
            foreach (JsonDocument doc in stream)
            {
                var ret = SearchObjectNode(doc.RootElement, name, kind);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }

        public static JsonElement? SearchObjectNode(JsonElement node, string name, JsonValueKind kind)
        {
            if (node.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in node.EnumerateObject())
                {
                    if (property.Name == name && property.Value.ValueKind == kind)
                    {
                        return property.Value;
                    }
                    if (property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array)
                    {
                        var ret = SearchObjectNode(property.Value, name, kind);
                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                }
            }
            if (node.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in node.EnumerateArray())
                {
                    var ret = SearchObjectNode(item, name, kind); 
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }
    }
}

