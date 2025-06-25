using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Parsing {
        /// <summary>Try converting the provided String into a JSON Object</summary>
        public static bool TryParseJSON<TO>( this string @string, [NotNullWhen(true)] out TO? value, JsonSerializerOptions? options = null ) {
            try {
                object? parsed = JsonSerializer.Deserialize<TO>(@string, options ?? new JsonSerializerOptions());
                if (parsed is TO @out) {
                    value = @out;
                    return true;
                }
            } catch (JsonException) {}
            
            value = default;
            return false;
        }
        
        /// <summary>Try converting a keyed value in the Object to another</summary>
        public static bool TryParseJSON<T>( this JsonObject that, string key, [NotNullWhen(true)] out T? value, JsonSerializerOptions? options = null ) {
            if ( that.TryGetPropertyValue(key, out object? obj) ) {
                if ( obj is T t ) {
                    value = t;
                    return true;
                }
                
                if ( obj is JsonObject jObject ) {
                    if ( jObject.Deserialize<T>(options) is T deserialized ) {
                        value = deserialized;
                        return true;
                    }
                } else if ( obj is JsonArray jArray ) {
                    if ( jArray.Deserialize<T>(options) is T deserialized ) {
                        value = deserialized;
                        return true;
                    }
                } else if ( obj is string @string ) {
                    return @string.TryParseJSON(out value, options);
                }
            }
            
            value = default;
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
