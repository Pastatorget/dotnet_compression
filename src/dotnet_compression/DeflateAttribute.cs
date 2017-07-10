using dotnet_compression.Resources;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Web.Http.Filters;

namespace dotnet_compression
{
    public class DeflateAttribute : ActionFilterAttribute
    {
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;
        public Int64 ByteThreshold { get; set; } = 5000;
        public Dictionary<string, string> Headers { get; set; }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = new byte[0];

            if (context.Response.Content != null)
            {
                var bytes = context.Response.Content.ReadAsByteArrayAsync().Result;

                if (bytes.Length < this.ByteThreshold)
                {
                    if (bytes != null)
                    {
                        content = CompressionHelper.DeflateBytes(bytes, this.CompressionLevel);
                    }

                    context.Response.Content = new ByteArrayContent(content);

                    if (this.Headers.Count > 0)
                    {
                        // Removes all header key values matching in this.Headers
                        foreach (var header in this.Headers)
                        {
                            if (context.Response.Content.Headers.Contains(header.Key))
                            {
                                // Removes header based on key
                                context.Response.Content.Headers.Remove(header.Key);

                                // Add updated header key and value
                                context.Response.Content.Headers.Add(header.Key, header.Value);
                            }
                        }
                    }
                    else
                    {
                        context.Response.Content.Headers.Remove("Content-Type");
                        context.Response.Content.Headers.Add("Content-Type", "application/json");
                    }

                    // For now we're forcing Content-encoding to deflate, since that's the compression method used
                    context.Response.Content.Headers.Add("Content-encoding", "deflate");
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
