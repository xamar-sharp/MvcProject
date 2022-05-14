using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.IO;
using System;
namespace MvcProject
{
    public class CompositeCompressionProvider:ICompressionProvider
    {
        public string EncodingName => "gzip,br";
        public bool SupportsFlush => true;
        public Stream CreateStream(Stream stream)
        {
            using(BrotliStream brotli = new BrotliStream(stream, CompressionLevel.Optimal))
            {
                return new GZipStream(brotli, CompressionLevel.Optimal);
            }
        }
    }
}
