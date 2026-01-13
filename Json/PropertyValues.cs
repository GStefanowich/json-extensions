using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class PropertyValues {
        /// <summary>Try to get a value from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonValue"/> value output</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out JsonValue? @out ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out JsonNode? node) && node is JsonValue value ) {
                @out = value;
                return true;
            }
            
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an array from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonArray"/> value output</param>
        /// <param name="tryParse">If the value is not a <see cref="T:System.Text.Json.Nodes.JsonArray"/> try to coerce the value</param>
        [Obsolete($"Deprecated but available for backward compatibility, replace {nameof(tryParse)} with {nameof(JsonSerializerOptions)}")]
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out JsonArray? @out, bool tryParse )
            => that.TryGetPropertyValue(key, out @out, tryParse ? JsonSerializerOptions.Default : null);
        
        /// <summary>Try to get an array from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonArray"/> value output</param>
        /// <param name="options">Parser options, if the value is not a <see cref="T:System.Text.Json.Nodes.JsonArray"/> it will be used to try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out JsonArray? @out, JsonSerializerOptions? options = null ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out JsonNode? node) ) {
                switch (node) {
                    case JsonArray array:
                        @out = array;
                        return true;
                    case JsonObject @object when options is not null:
                        return @object.TryConvertToArray(out @out, options: options);
                    case JsonValue value when options is not null:
                        if (value.TryGetValue(out string? @string))
                            return @string.TryParseJSON(out @out, options);
                        break;
                }
            }
            
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an object from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonObject"/> value output</param>
        /// <param name="tryParse">If the value is not a <see cref="T:System.Text.Json.Nodes.JsonObject"/> try to coerce the value</param>
        [Obsolete($"Deprecated but available for backward compatibility, replace {nameof(tryParse)} with {nameof(JsonSerializerOptions)}")]
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out JsonObject? @out, bool tryParse )
            => that.TryGetPropertyValue(key, out @out, tryParse ? JsonSerializerOptions.Default : null);
        
        /// <summary>Try to get an object from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonObject"/> value output</param>
        /// <param name="options">Parser options, if the value is not a <see cref="T:System.Text.Json.Nodes.JsonObject"/> it will be used to try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out JsonObject? @out, JsonSerializerOptions? options = null ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out JsonNode? node) ) {
                switch (node) {
                    case JsonObject @object:
                        @out = @object;
                        return true;
                    case JsonValue value when options is not null:
                        if ( value.TryGetValue(out string? @string) ) {
                            return @string.TryParseJSON(out @out, options);
                        }
                        break;
                }
            }
            
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an object from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="object"/> value output</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out object? @out ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out JsonNode? node) ) {
                if ( node is JsonArray array ) {
                    @out = array;
                    return true;
                }
                
                if ( node is JsonObject @object ) {
                    @out = @object;
                    return true;
                }
                
                if ( node is JsonValue jValue ) {
                    @out = jValue.GetValue<object>();
                    
                    // Get the underlying value of the element
                    if ( @out is JsonElement element ) {
                        @out = element.ValueKind switch {
                            JsonValueKind.Object => JsonObject.Create(element),
                            JsonValueKind.Array => JsonArray.Create(element),
                            JsonValueKind.String => element.GetString(),
                            JsonValueKind.Number => element.GetInt64(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Undefined or JsonValueKind.Null => null,
                            _ => @out,
                        };
                    }
                    
                    return @out is not null;
                }
            }
            
            @out = null;
            return false;
        }
        
        /// <summary>Try to get a string from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Text.Json.Nodes.JsonObject"/> value output</param>
        /// <param name="strict">If the value must strictly a <see langword="string"/> (Opposite to usual tryParse)</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, [NotNullWhen(true)] out string? @out, bool strict = false ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( !that.TryGetPropertyValue(key, out object? @object) )
                @out = null;
            else {
                @out = @object switch {
                    string @string => @string,
                    bool @bool when !strict => @bool.ToString(),
                    int @int when !strict => @int.ToString(),
                    uint @uint when !strict => @uint.ToString(),
                    long @long when !strict => @long.ToString(),
                    ulong @ulong when !strict => @ulong.ToString(),
                    DateTime time when !strict => time.ToString(CultureInfo.InvariantCulture),
                    DateTimeOffset time when !strict => time.ToString(CultureInfo.InvariantCulture),
                    _ => null,
                };
            }
            
            return @out is not null;
        }
        
        /// <summary>Try to get a bool from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="bool"/> value output</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out bool @out ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out object? @object) && @object is bool @bool ) {
                @out = @bool;
                return true;
            }
            
            @out = false;
            return false;
        }
        
        /// <summary>Try to get a long from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="long"/> value output</param>
        /// <param name="tryParse">If the value is not a <see langword="long"/> try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out long @out, bool tryParse = false ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out object? @object) ) {
                switch (@object) {
                    case int @int:
                        @out = @int;
                        return true;
                    case long @long:
                        @out = @long;
                        return true;
                    case string @string when tryParse:
                        return long.TryParse(@string, out @out);
                }
            }
            
            @out = 0L;
            return false;
        }
        
        /// <summary>Try to get a long from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="ulong"/> value output</param>
        /// <param name="tryParse">If the value is not a <see langword="ulong"/> try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out ulong @out, bool tryParse = false ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if (that.TryGetPropertyValue(key, out object? @object)) {
                switch (@object) {
                    case uint @uint:
                        @out = @uint;
                        return true;
                    case int @int and >= 0:
                        @out = (ulong)@int;
                        return true;
                    case ulong @ulong:
                        @out = @ulong;
                        return true;
                    case long @long and >= 0:
                        @out = (ulong)@long;
                        return true;
                    case string @string when tryParse:
                        return ulong.TryParse(@string, out @out);
                }
            }
            
            @out = 0ul;
            return false;
        }
        
        /// <summary>Try to get an int from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="int"/> value output</param>
        /// <param name="tryParse">If the value is not a <see langword="int"/> try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out int @out, bool tryParse = false ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out object? @object) ) {
                switch (@object) {
                    case int @int:
                        @out = @int;
                        return true;
                    case long @long:
                        @out = (int) @long;
                        return true;
                    case string @string when tryParse:
                        return int.TryParse(@string, out @out);
                }
            }
            
            @out = 0;
            return false;
        }
        
        /// <summary>Try to get an int from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see langword="uint"/> value output</param>
        /// <param name="tryParse">If the value is not a <see langword="uint"/> try to coerce the value</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out uint @out, bool tryParse = false ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out object? @object) ) {
                switch (@object) {
                    case uint @uint:
                        @out = @uint;
                        return true;
                    case int @int and >= 0:
                        @out = (uint)@int;
                        return true;
                    case ulong @ulong and <= uint.MaxValue:
                        @out = (uint)@ulong;
                        return true;
                    case long @long and >= 0 and <= uint.MaxValue:
                        @out = (uint)@long;
                        return true;
                    case string @string when tryParse:
                        return uint.TryParse(@string, out @out);
                }
            }
            
            @out = 0u;
            return false;
        }
        
        /// <summary>Try to get a GUID from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.Guid"/> value output</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out Guid @out ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out string? @string) && Guid.TryParse(@string, out @out) )
                return true;
            
            @out = Guid.Empty;
            return false;
        }
        
        /// <summary>Try to get a DateTime from the object</summary>
        /// <param name="that"><see cref="T:System.Text.Json.Nodes.JsonObject"/> to try coercing a property from</param>
        /// <param name="key">The property key to read</param>
        /// <param name="out"><see cref="T:System.DateTimeOffset"/> value output</param>
        public static bool TryGetPropertyValue( this JsonObject that, string key, out DateTimeOffset @out ) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            
            if ( that.TryGetPropertyValue(key, out object? @object) ) {
                switch (@object) {
                    case DateTimeOffset dateTimeOffset:
                        @out = dateTimeOffset;
                        return true;
                    case string @string when DateTimeOffset.TryParse(@string, out DateTimeOffset dateTimeOffset):
                        @out = dateTimeOffset;
                        return true;
                    case DateTime dateTime:
                        @out = dateTime.ToUniversalTime();
                        return true;
                    case string @string when DateTime.TryParse(@string, out DateTime dateTime):
                        @out = dateTime.ToUniversalTime();
                        return true;
                }
            }
            
            @out = default;
            return false;
        }
    }
}
