using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Coercion {
        /// <summary>Convert the given <see cref="JsonObject"/> to a <see cref="JsonArray"/></summary>
        /// <remarks>Will convert the <see cref="JsonObject"/> to a <see cref="JsonArray"/> if the Keys of the Object are sequential Numbers</remarks>
        public static bool TryConvertToArray( this JsonObject @object, [NotNullWhen(true)] out JsonArray? @out, bool zeroIndex = true, JsonSerializerOptions? options = null ) {
            _ = @object ?? throw new ArgumentNullException(nameof(@object));
            
            int pos = zeroIndex ? 0 : 1;
            int count = @object.Count;
            
            while ( @object.TryGetPropertyValue($"{pos}", out _) )
                pos++;
            
            if ( pos == count ) {
                @out = new JsonArray(@object.CloneValues(options).ToArray());
                return true;
            }
            
            @out = null;
            return false;
        }
    }
}
