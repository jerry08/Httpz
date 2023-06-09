using System;
using System.Net.Http;

namespace Httpz;

internal class HttpClientFactory : IHttpClientFactory
{
    private readonly Func<HttpClient> _httpClientFunc;

    public HttpClientFactory(Func<HttpClient> httpClientFunc)
    {
        _httpClientFunc = httpClientFunc;
    }

    public HttpClientFactory() : this(() => new())
    {
    }

    public HttpClient CreateClient() => _httpClientFunc();
}