using System.IO;
using System.IO.Compression;
using System.Text;

namespace MobileFramework.Core.Serialization
{
    /// <summary>
    /// Serializzazione binaria per salvataggi grandi: JSON compresso con GZip.
    /// Pensato per essere sostituito da MessagePack senza cambiare i chiamanti
    /// se servisse più throughput.
    /// </summary>
    public static class BinarySerializer
    {
        public static byte[] Serialize(object value)
        {
            var raw = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(raw, 0, raw.Length);
            }

            return output.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            using var input = new MemoryStream(data);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }
    }
}
