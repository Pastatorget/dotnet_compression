using System.IO;
using System.IO.Compression;

namespace dotnet_compression.Resources
{
    public class CompressionHelper
    {
        public static byte[] DeflateBytes(byte[] str, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new DeflateStream(output, compressionLevel))
                {
                    compressor.Write(str, 0, str.Length);
                }

                return output.ToArray();
            }
        }

        public static byte[] GzipBytes(byte[] str, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new GZipStream(output, compressionLevel))
                {
                    compressor.Write(str, 0, str.Length);
                }

                return output.ToArray();
            }
        }
    }
}
