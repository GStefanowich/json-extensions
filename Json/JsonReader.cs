using System.Text.Json;

namespace TheElm.Text.Json {
    public static class JsonReader {
        public static T? Load<T>( string path, JsonSerializerOptions? options = null )
            => JsonReader.Load<T>(new FileInfo(path), options);
        
        public static T? Load<T>( FileInfo file, JsonSerializerOptions? options = null ) {
            options ??= JsonSerializerOptions.Default;
            
            using ( FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: options.DefaultBufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous) ) {
                return JsonSerializer.Deserialize<T>(fs, options);
            }
        }
        
        public static Task<T?> LoadAsync<T>( string path, JsonSerializerOptions? options = null, CancellationToken cancellation = default )
            => JsonReader.LoadAsync<T>(new FileInfo(path), options, cancellation);
        
        public static async Task<T?> LoadAsync<T>( FileInfo file, JsonSerializerOptions? options = null, CancellationToken cancellation = default ) {
            options ??= JsonSerializerOptions.Default;
            
            await using ( FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: options.DefaultBufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous) ) {
                return await JsonSerializer.DeserializeAsync<T>(fs, options, cancellation);
            }
        }
    }
}
