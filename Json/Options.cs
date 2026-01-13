using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheElm.Text.Json {
    public static class Options {
        /// <summary>
        /// Get the SerializerOptions <see cref="JsonDocumentOptions"/>
        /// </summary>
        public static JsonDocumentOptions GetDocumentOptions( this JsonSerializerOptions options ) {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            
            return new JsonDocumentOptions {
                AllowTrailingCommas = options.AllowTrailingCommas,
                CommentHandling = options.ReadCommentHandling,
                MaxDepth = options.MaxDepth
            };
        }
        
        /// <summary>
        /// Get the SerializerOptions <see cref="JsonNodeOptions"/>
        /// </summary>
        public static JsonNodeOptions GetNodeOptions( this JsonSerializerOptions options ) {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            
            return new JsonNodeOptions {
                PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive
            };
        }
        
        /// <summary>
        /// Get the SerializerOptions <see cref="JsonReaderOptions"/>
        /// </summary>
        public static JsonReaderOptions GetReaderOptions( this JsonSerializerOptions options ) {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            
            return new JsonReaderOptions {
                AllowTrailingCommas = options.AllowTrailingCommas,
                CommentHandling = options.ReadCommentHandling,
                MaxDepth = options.MaxDepth
            };
        }
        
        /// <summary>
        /// Get the SerializerOptions <see cref="JsonWriterOptions"/>
        /// </summary>
        public static JsonWriterOptions GetWriterOptions( this JsonSerializerOptions options ) {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            
            return new JsonWriterOptions {
                Encoder = options.Encoder,
                Indented = options.WriteIndented,
                #if !DEBUG
                SkipValidation = true
                #endif
            };
        }
        
        /// <summary>
        /// Duplicate a JsonSerializer options
        /// </summary>
        public static JsonSerializerOptions Clone( this JsonSerializerOptions options )
            => new(options);
    }
}
