using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Duplication {
        public static T? Clone<T>( this T node, JsonSerializerOptions? options = null ) where T : JsonNode
            => node.CloneCore(0, options ?? JsonSerializerOptions.Default);
        
        private static T? CloneCore<T>( this T node, int depth, JsonSerializerOptions options ) where T : JsonNode
            => node.CloneCore(depth, options, options.GetNodeOptions());
        
        private static T? CloneCore<T>( this T that, int depth, JsonSerializerOptions serializerOptions, JsonNodeOptions nodeOptions ) where T : JsonNode {
            if ( that is JsonObject obj ) {
                JsonObject other = new(nodeOptions);
                
                // Only populate the object if it doesn't exceed the max depth
                if ( depth < GetMaxDepth() ) {
                    foreach ( (string key, JsonNode? node) in obj ) {
                        other[key] = node?.CloneCore(depth + 1, serializerOptions, nodeOptions);
                    }
                }
                
                return other as T;
            }
            
            if ( that is JsonArray arr ) {
                JsonArray other = new(nodeOptions);
                
                // Only populate the array if it doesn't exceed the max depth
                if ( depth < GetMaxDepth() ) {
                    foreach ( JsonNode? node in arr ) {
                        other.Add(node?.CloneCore(depth + 1, serializerOptions, nodeOptions));
                    }
                }
                
                return other as T;
            }
            
            if ( that is JsonValue val ) {
                if ( val.TryGetValue(out object? value) )
                    return JsonValue.Create(value, nodeOptions) as T;
                
                throw new Exception("");
            }
            
            throw new Exception($"Could not clone type '{typeof(T)}'");
            
            int GetMaxDepth() {
                int max = serializerOptions.MaxDepth;
                return max is 0 ? 64 : max;
            }
        }
    }
}
