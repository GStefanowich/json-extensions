using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TheElm.Text.Json {
    public static class JsonWriter {
        public static Task SaveAsync<T>( string path, T value, JsonSerializerOptions? options = null, CancellationToken cancellation = default )
            => JsonWriter.SaveAsync(new FileInfo(path), value, options, cancellation);
        
        public static async Task SaveAsync<T>( FileInfo file, T value, JsonSerializerOptions? options = null, CancellationToken cancellation = default ) {
            options ??= JsonSerializerOptions.Default;
            
            await using ( FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: options.DefaultBufferSize, FileOptions.SequentialScan | FileOptions.Asynchronous) ) {
                await JsonSerializer.SerializeAsync(fs, value, options, cancellation);
            }
        }
    }
}
