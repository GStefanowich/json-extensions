using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Parsing {
        /// <summary>Try converting the provided String into a JSON Object</summary>
        public static bool TryParseJSON<TO>( this string @string, [NotNullWhen(true)] out TO? jObject, JsonSerializerOptions? options = null ) {
            try {
                object? parsed = JsonSerializer.Deserialize<TO>(@string, options ?? new JsonSerializerOptions());
                if (parsed is TO @out) {
                    jObject = @out;
                    return true;
                }
            } catch (JsonException) {}
            
            jObject = default;
            return false;
        }
        
        /// <summary>Try converting the provided String into a JSON Object</summary>
        public static bool TryParseJSON( this string @string, [NotNullWhen(true)] out JsonObject? jObject, JsonSerializerOptions? options = null ) {
            try {
                JsonDocument document = JsonDocument.Parse(@string, options?.GetDocumentOptions() ?? default);
                if (document.RootElement.ValueKind is JsonValueKind.Object) {
                    jObject = JsonObject.Create(document.RootElement);
                    return jObject is not null;
                }
            } catch (JsonException) {}
            
            jObject = null;
            return false;
        }
        
        /// <summary>Try converting the provided String into a JSON Array</summary>
        public static bool TryParseJSON( this string @string, [NotNullWhen(true)] out JsonArray? jArray, JsonSerializerOptions? options = null ) {
            try {
                JsonDocument document = JsonDocument.Parse(@string, options?.GetDocumentOptions() ?? default);
                if (document.RootElement.ValueKind is JsonValueKind.Array) {
                    jArray = JsonArray.Create(document.RootElement);
                    return jArray is not null;
                }
            } catch (JsonException) {}
            
            jArray = null;
            return false;
        }
        
        /// <summary>Try converting the provided String into a JSON Array</summary>
        public static bool TryParseJSON( this string @string, [NotNullWhen(true)] out JsonNode? jNode, JsonSerializerOptions? options = null ) {
            try {
                jNode = JsonNode.Parse(@string, options?.GetNodeOptions());
                return jNode is not null;
            } catch (JsonException) {}
            
            jNode = null;
            return false;
        }
    }
}
