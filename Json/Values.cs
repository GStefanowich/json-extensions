using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Values {
        /// <summary>Get the value of a JObject as a specific type</summary>
        public static T? GetValue<T>( this JsonObject that, string key ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            Type type = typeof(T);
            
            // If the value we're getting is a JObject
            if ( type == typeof(JsonObject) )
                return that.TryGetPropertyValue(key, out JsonObject? @object)
                    && @object is T val ? val : default;
            
            // If the value we're getting is a JArray
            if ( type == typeof(JsonArray) )
                return that.TryGetPropertyValue(key, out JsonArray? array)
                    && array is T val ? val : default;
            
            // Fallback to JValue
            return that.TryGetPropertyValue(key, out JsonValue? value)
                && value.TryGetValue(out T? @out) ? @out : default;
        }
        
        /// <summary>Get the value of a Node as a specific type</summary>
        private static T? GetValue<T>( JsonNode node, JsonSerializerOptions? options = null ) {
            // If the value we're getting is a JArray
            if ( node is T type )
                return type;
            
            // Fallback to JValue
            return node switch {
                JsonValue value => value.TryGetValue(out T? @out) ? @out : default,
                _ => node.Deserialize<T>(options),
            };
        }
        
        /// <summary>Get all of the Keys of a JsonObject</summary>
        public static IEnumerable<string> GetKeys( this JsonObject @object )
            => @object.Select(pair => pair.Key);
        
        /// <summary>Get all of the Values of a JsonObject that are a specific type</summary>
        public static IEnumerable<T> GetValues<T>( this JsonObject @object, JsonSerializerOptions? options = null ) {
            foreach ( KeyValuePair<string, JsonNode?> pair in @object ) {
                if ( pair.Value is null )
                    continue;
                
                if ( Values.GetValue<T>(pair.Value, options) is T value )
                    yield return value;
            }
        }
        
        /// <summary>Get all of the Values of a JsonObject</summary>
        public static IEnumerable<JsonNode?> GetValues( this JsonObject @object )
            => @object.Select(pair => pair.Value);
        
        /// <summary>Get all of the Values of a JsonArray that are a specific type</summary>
        public static IEnumerable<T> GetValues<T>( this JsonArray array, JsonSerializerOptions? options = null ) {
            foreach ( JsonNode? node in array ) {
                if ( node is null )
                    continue;
                
                if ( Values.GetValue<T>(node, options) is T value )
                    yield return value;
            }
        }
        
        /// <summary>Get all of the Values of a JsonObject</summary>
        public static IEnumerable<JsonNode?> CloneValues( this JsonObject @object, JsonSerializerOptions? options = null )
            => @object.Select(pair => pair.Value?.Clone(options));
    }
}
