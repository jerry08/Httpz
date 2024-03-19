using System;
using System.Net.Http;

namespace Httpz;

internal class HttpClientFactory(Func<HttpClient> httpClientFunc) : IHttpClientFactory
{
    public HttpClientFactory()
        : this(() => new()) { }

    public HttpClient CreateClient() => httpClientFunc();
}
