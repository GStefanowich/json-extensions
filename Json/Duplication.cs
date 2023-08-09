using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Duplication {
        public static T? Clone<T>(this T that) where T : JsonNode {
            if (that is JsonObject jObject) {
                JsonObject other = new();
                foreach ((string key, JsonNode? node) in jObject) {
                    other[key] = node?.Clone();
                }
                return other as T;
            }
            
            if (that is JsonArray jArray) {
                JsonArray other = new();
                foreach (JsonNode? node in jArray) {
                    other.Add(node?.Clone());
                }
                return other as T;
            }
            
            if (that is JsonValue jValue) {
                if (jValue.TryGetValue(out object? value))
                    return JsonValue.Create(value) as T;
                throw new Exception("");
            }
            
            throw new Exception($"Could not clone type '{typeof(T)}'");
        }
    }
}
