using dotnet_compression.Resources;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Web.Http.Filters;

namespace dotnet_compression
{
    public class GzipAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Let's you control the compression level of the body response to the client.
        /// Will default to fastest if not given.
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;

        /// <summary>
        /// To avoid perfomance issues, compression won't be triggered
        /// if the response is below 5kb by default. You can change this to
        /// any positive value you want
        /// </summary>
        public Int64 ByteThreshold { get; set; } = 5000;

        /// <summary>
        /// Adds support for define custom return head attributes to client
        /// </summary>
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
                        content = CompressionHelper.GzipBytes(bytes, this.CompressionLevel);
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

                    // For now we're forcing Content-encoding to gzip, since that's the compression method used
                    context.Response.Content.Headers.Add("Content-encoding", "gzip");
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
