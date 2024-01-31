using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class PropertyValues {
        /// <summary>Try to get a value from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, [NotNullWhen(true)] out JsonValue? @out) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out JsonNode? token) && token is JsonValue jValue) {
                @out = jValue;
                return true;
            }
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an array from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, [NotNullWhen(true)] out JsonArray? @out, bool tryParse = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out JsonNode? node)) {
                switch (node, tryParse) {
                    case (JsonArray array, _):
                        @out = array;
                        return true;
                    case (JsonObject @object, true):
                        return @object.TryConvertToArray(out @out);
                    case (JsonValue value, true):
                        if (value.TryGetValue(out string? @string))
                            return @string.TryParseJSON(out @out);
                        break;
                }
            }
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an object from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, [NotNullWhen(true)] out JsonObject? @out, bool tryParse = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out JsonNode? token)) {
                switch (token, tryParse) {
                    case (JsonObject jObject, _):
                        @out = jObject;
                        return true;
                    case (JsonValue value, true):
                        if (value.TryGetValue(out string? @string))
                            return @string.TryParseJSON(out @out);
                        break;
                }
            }
            @out = null;
            return false;
        }
        
        /// <summary>Try to get an object from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, [NotNullWhen(true)] out object? @out) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out JsonValue? jValue)) {
                @out = jValue.GetValue<object>();
                
                // Get the underlying value of the element
                if (@out is JsonElement element) {
                    switch (element.ValueKind) {
                        case JsonValueKind.Object:
                            @out = JsonObject.Create(element);
                            break;
                        case JsonValueKind.Array:
                            @out = JsonArray.Create(element);
                            break;
                        case JsonValueKind.String:
                            @out = element.GetString();
                            break;
                        case JsonValueKind.Number:
                            @out = element.GetInt64();
                            break;
                        case JsonValueKind.True:
                            @out = true;
                            break;
                        case JsonValueKind.False:
                            @out = false;
                            break;
                        case JsonValueKind.Undefined:
                        case JsonValueKind.Null:
                            @out = null;
                            break;
                    }
                }
                
                return @out is not null;
            }
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a string from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, [NotNullWhen(true)] out string? @out, bool strict = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object)) {
                switch (@object) {
                    case string @string: {
                        @out = @string;
                        return true;
                    }
                    case bool @bool when !strict: {
                        @out = @bool.ToString();
                        return true;
                    }
                    case int @int when !strict: {
                        @out = @int.ToString();
                        return true;
                    }
                    case uint @uint when !strict: {
                        @out = @uint.ToString();
                        return true;
                    }
                    case long @long when !strict: {
                        @out = @long.ToString();
                        return true;
                    }
                    case ulong @ulong when !strict: {
                        @out = @ulong.ToString();
                        return true;
                    }
                    case DateTime time when !strict: {
                        @out = time.ToString(CultureInfo.InvariantCulture);
                        return true;
                    }
                    case DateTimeOffset time when !strict: {
                        @out = time.ToString(CultureInfo.InvariantCulture);
                        return true;
                    }
                }
            }
            
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a bool from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out bool @out) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object) && @object is bool @bool) {
                @out = @bool;
                return true;
            }
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a long from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out long @out, bool tryParse = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object)) {
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
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a long from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out ulong @out, bool tryParse = false) {
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
            @out = default;
            return false;
        }
        
        /// <summary>Try to get an int from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out int @out, bool tryParse = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object)) {
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
            @out = default;
            return false;
        }
        
        /// <summary>Try to get an int from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out uint @out, bool tryParse = false) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object)) {
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
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a GUID from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out Guid @out) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out string? @string) && Guid.TryParse(@string, out @out))
                return true;
            @out = default;
            return false;
        }
        
        /// <summary>Try to get a DateTime from the object</summary>
        public static bool TryGetPropertyValue(this JsonObject that, string key, out DateTimeOffset @out) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            if (that.TryGetPropertyValue(key, out object? @object)) {
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
