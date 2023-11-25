using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Mexc.Net.Objects.Options;
using Mexc.Net.Clients;
using Mexc.Net.Interfaces.Clients;

namespace Mexc.Net;

/// <summary>
/// Helper methods
/// </summary>
public static class MexcHelpers
{
    internal static bool IsIn<T>(this T @this, params T[] values)
    {
        return Array.IndexOf(values, @this) != -1;
    }

    internal static bool IsNotIn<T>(this T @this, params T[] values)
    {
        return Array.IndexOf(values, @this) == -1;
    }
    /// <summary>
    /// Validate the string is a valid Mexc symbol.
    /// </summary>
    /// <param name="symbolString">string to validate</param> 
    public static void ValidateMexcSymbol(this string symbolString)
    {
        if (string.IsNullOrEmpty(symbolString))
            throw new ArgumentException("Symbol is not provided");

        if(!Regex.IsMatch(symbolString, "^([A-Z|a-z|0-9]{5,})$"))
            throw new ArgumentException($"{symbolString} is not a valid Mexc symbol. Should be [BaseAsset][QuoteAsset], e.g. BTCUSDT");
    }
    /// <summary>
    /// Add the IMexcClient and IMexcSocketClient to the sevice collection so they can be injected
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="defaultRestOptionsDelegate">Set default options for the rest client</param>
    /// <param name="defaultSocketOptionsDelegate">Set default options for the socket client</param>
    /// <param name="socketClientLifeTime">The lifetime of the IMexcSocketClient for the service collection. Defaults to Singleton.</param>
    /// <returns></returns>
    public static IServiceCollection AddMexc(
        this IServiceCollection services,
        Action<MexcRestOptions>? defaultRestOptionsDelegate = null,
        Action<MexcSocketOptions>? defaultSocketOptionsDelegate = null,
        ServiceLifetime? socketClientLifeTime = null)
    {
        var restOptions = MexcRestOptions.Default.Copy();

        if (defaultRestOptionsDelegate != null)
        {
            defaultRestOptionsDelegate(restOptions);
            MexcRestClient.SetDefaultOptions(defaultRestOptionsDelegate);
        }

        if (defaultSocketOptionsDelegate != null)
            MexcSocketClient.SetDefaultOptions(defaultSocketOptionsDelegate);

        services.AddHttpClient<IMexcRestClient, MexcRestClient>(options =>
        {
            options.Timeout = restOptions.RequestTimeout;
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            if (restOptions.Proxy != null)
            {
                handler.Proxy = new WebProxy
                {
                    Address = new Uri($"{restOptions.Proxy.Host}:{restOptions.Proxy.Port}"),
                    Credentials = restOptions.Proxy.Password == null ? null : new NetworkCredential(restOptions.Proxy.Login, restOptions.Proxy.Password)
                };
            }
            return handler;
        });

        //services.AddSingleton<IMexcOrderBookFactory, MexcOrderBookFactory>();
        services.AddTransient<IMexcRestClient, MexcRestClient>();
        if (socketClientLifeTime == null)
            services.AddSingleton<IMexcSocketClient, MexcSocketClient>();
        else
            services.Add(new ServiceDescriptor(typeof(IMexcSocketClient), typeof(MexcSocketClient), socketClientLifeTime.Value));
        return services;
    }
}