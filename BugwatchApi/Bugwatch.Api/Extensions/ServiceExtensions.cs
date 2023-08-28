using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace Bugwatch.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddResponseCompressionService(this IServiceCollection services)
    {
        services.AddResponseCompression(opts => { opts.EnableForHttps = true; });
        services.Configure<GzipCompressionProviderOptions>(opts => { opts.Level = CompressionLevel.Fastest; });
        services.Configure<BrotliCompressionProviderOptions>(opts => { opts.Level = CompressionLevel.Fastest; });

        return services;
    }
}