using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SatisfactoryModelerAddons.StarRupture
{
    public readonly struct JNode
    {
        private readonly JsonElement? _node;

        public JNode(JsonElement node) => _node = node;
        public JNode(JsonElement? node) => _node = node;

        public static implicit operator JNode(JsonElement e) => new(e);
        public static implicit operator JNode(JsonElement? e) => new(e);

        public JNode this[string name] => _node is JsonElement e && e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var ret) ? new(ret) : new((JsonElement?)null);

        public JNode this[int index] => _node is JsonElement e && e.ValueKind == JsonValueKind.Array && index >= 0 && index < e.GetArrayLength() ? new(e[index]) : new((JsonElement?)null);

        public string? Str() => _node?.GetString();

        public int? Int() => _node?.GetInt32();
        public void Debug(string filename)
        {
            if (_node == null)
            {
                return;
            }
            File.WriteAllText(filename, JsonSerializer.Serialize(_node, new JsonSerializerOptions { WriteIndented = true }));
        }

        public IEnumerable<JNode> EnumerateArray()
        {
            if (_node is JsonElement e && e.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in e.EnumerateArray())
                {
                    yield return new JNode(item);
                }
            }
        }

        public struct JNodeProperty
        {
            public string Name { get; }
            public JNode Value { get; }

            public JNodeProperty(string name, JNode value) : this()
            {
                Name = name;
                Value = value;
            }
        }
        public IEnumerable<JNodeProperty> EnumerateObject()
        {
            if (_node is JsonElement e && e.ValueKind == JsonValueKind.Object)
            {
                foreach (var p in e.EnumerateObject())
                {
                    yield return new JNodeProperty(p.Name, new JNode(p.Value));
                }
            }
        }

        public JNode Walk(JNode source,string fromKey)
        {
            if (_node is JsonElement n && n.ValueKind == JsonValueKind.String)
            {
                var current = source._node;
                var parts = n.GetString()?.Split(':') ?? [];
                var keyIdx = Array.IndexOf(parts, fromKey);
                foreach (var idx in parts[(keyIdx + 1)..])
                {
                    current = WalkUnit(current, idx);
                }
                return new JNode(current);
            }
            return this;
        }
        private JsonElement? WalkUnit(JsonElement? node, string index)
        {
            if (node == null)
            {
                return null;
            }
            if (node is JsonElement arr && arr.ValueKind == JsonValueKind.Array && int.TryParse(index, out var idx) && idx >= 0 && idx < arr.GetArrayLength())
            {
                return arr[idx];
            }
            if (node is JsonElement obj && obj.ValueKind == JsonValueKind.Object)
            {
                return obj.GetProperty(index);
            }
            return null;
        }
    }
}
