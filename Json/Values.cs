using System;
using System.Collections.Generic;
using System.Linq;
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
        private static T? GetValue<T>( JsonNode node ) {
            // If the value we're getting is a JArray
            if ( node is T type )
                return type;
            
            // Fallback to JValue
            return node is JsonValue value
                && value.TryGetValue(out T? @out) ? @out : default;
        }
        
        /// <summary>Get all of the Keys of a JsonObject</summary>
        public static IEnumerable<string> GetKeys( this JsonObject that )
            => that.Select(pair => pair.Key);
        
        public static IEnumerable<T> GetValues<T>( this JsonObject that ) {
            foreach (KeyValuePair<string, JsonNode?> pair in that) {
                if (pair.Value is null)
                    continue;
                if (Values.GetValue<T>(pair.Value) is T value)
                    yield return value;
            }
        }
        
        /// <summary>Get all of the Values of a JsonObject</summary>
        public static IEnumerable<JsonNode?> GetValues( this JsonObject that )
            => that.Select(pair => pair.Value);
        
        /// <summary>Get all of the Values of a JsonObject</summary>
        public static IEnumerable<JsonNode?> CloneValues( this JsonObject that, JsonSerializerOptions? options = null )
            => that.Select(pair => pair.Value?.Clone(options));
    }
}
