﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Httpz.Utils.Extensions;

public static class HttpExtensions
{
    public static bool GetAllowAutoRedirect(this HttpClient http)
    {
        var handlerField = Array.Find(http.GetType().BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance),
            x => x.Name.Contains("handler"))!;

        var handlerVal = handlerField.GetValue(http)!;

        var allowAutoRedirectProp = handlerVal.GetType().GetProperty("AllowAutoRedirect", BindingFlags.Public | BindingFlags.Instance)!;
        var allowAutoRedirectPropVal = allowAutoRedirectProp.GetValue(handlerVal)!;

        return bool.Parse(allowAutoRedirectPropVal.ToString()!);
    }

    public static void SetAllowAutoRedirect(
        this HttpClient http,
        bool value)
    {
        var handlerField = Array.Find(http.GetType().BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance),
            x => x.Name.Contains("handler"))!;

        var handlerVal = handlerField.GetValue(http)!;

        var allowAutoRedirectProp = handlerVal.GetType().GetProperty("AllowAutoRedirect", BindingFlags.Public | BindingFlags.Instance)!;
        allowAutoRedirectProp.SetValue(handlerVal, value);
    }

    public static async ValueTask<HttpResponseMessage> HeadAsync(
        this HttpClient http,
        string requestUri,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default)
        => await http.HeadAsync(new Uri(requestUri), headers, cancellationToken);

    public static async ValueTask<HttpResponseMessage> HeadAsync(
        this HttpClient http,
        Uri requestUri,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

        for (var i = 0; i < headers.Count; i++)
        {
            request.Headers.TryAddWithoutValidation(
                headers.ElementAt(i).Key,
                headers.ElementAt(i).Value
            );
        }

        return await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
    }

    public static async ValueTask<HttpResponseMessage> HeadAsync(
        this HttpClient http,
        string requestUri,
        CancellationToken cancellationToken = default)
        => await http.HeadAsync(new Uri(requestUri), cancellationToken);

    public static async ValueTask<HttpResponseMessage> HeadAsync(
        this HttpClient http,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
        return await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
    }

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        string requestUri,
        CancellationToken cancellationToken = default)
        => await http.GetStreamAsync(new Uri(requestUri), cancellationToken);

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        return await http.GetStreamAsync(requestUri, null, null, true, cancellationToken);
    }

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        string requestUri,
        long? from = null,
        long? to = null,
        CancellationToken cancellationToken = default)
        => await http.GetStreamAsync(new Uri(requestUri), from, to, cancellationToken);

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        Uri requestUri,
        long? from = null,
        long? to = null,
        CancellationToken cancellationToken = default)
    {
        return await http.GetStreamAsync(requestUri, from, to, true, cancellationToken);
    }

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        string requestUri,
        long? from = null,
        long? to = null,
        bool ensureSuccess = true,
        CancellationToken cancellationToken = default)
        => await http.GetStreamAsync(new Uri(requestUri), from, to, ensureSuccess, cancellationToken);

    public static async ValueTask<Stream> GetStreamAsync(
        this HttpClient http,
        Uri requestUri,
        long? from = null,
        long? to = null,
        bool ensureSuccess = true,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Range = new RangeHeaderValue(from, to);

        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (ensureSuccess)
            response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static async ValueTask<long?> TryGetContentLengthAsync(
        this HttpClient http,
        string requestUri,
        bool ensureSuccess = true,
        CancellationToken cancellationToken = default)
        => await http.TryGetContentLengthAsync(new Uri(requestUri), ensureSuccess, cancellationToken);

    public static async ValueTask<long?> TryGetContentLengthAsync(
        this HttpClient http,
        Uri requestUri,
        bool ensureSuccess = true,
        CancellationToken cancellationToken = default)
    {
        var response = await http.HeadAsync(requestUri, cancellationToken);

        if (ensureSuccess)
            response.EnsureSuccessStatusCode();

        return response.Content.Headers.ContentLength;
    }

    public static async ValueTask<string> GetAsync(
        this HttpClient http,
        string url,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        string url,
        CancellationToken cancellationToken = default)
        => await http.PostAsync(new Uri(url), cancellationToken);

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        string url,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default)
        => await http.PostAsync(new Uri(url), headers, cancellationToken);

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        Uri uri,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        for (var i = 0; i < headers.Count; i++)
        {
            request.Headers.TryAddWithoutValidation(
                headers.ElementAt(i).Key,
                headers.ElementAt(i).Value
            );
        }

        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        string url,
        Dictionary<string, string> headers,
        HttpContent content,
        CancellationToken cancellationToken = default)
        => await http.PostAsync(new Uri(url), headers, content, cancellationToken);

    public static async ValueTask<string> PostAsync(
        this HttpClient http,
        Uri uri,
        Dictionary<string, string> headers,
        HttpContent content,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        for (var i = 0; i < headers.Count; i++)
        {
            request.Headers.TryAddWithoutValidation(
                headers.ElementAt(i).Key,
                headers.ElementAt(i).Value
            );
        }

        request.Content = content;

        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async ValueTask<long> GetFileSizeAsync(
        this HttpClient http,
        string url,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
        => await http.GetFileSizeAsync(new Uri(url), headers, cancellationToken);

    public static async ValueTask<long> GetFileSizeAsync(
        this HttpClient http,
        Uri uri,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Head, uri);
        for (var i = 0; i < headers?.Count; i++)
        {
            request.Headers.TryAddWithoutValidation(
                headers.ElementAt(i).Key,
                headers.ElementAt(i).Value
            );
        }

        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})." +
                Environment.NewLine +
                "Request:" +
                Environment.NewLine +
                request
            );
        }

        return response.Content.Headers.ContentLength ?? 0;
    }

    public static async ValueTask<string> ExecuteAsync(
        this HttpClient http,
        string url,
        CancellationToken cancellationToken = default)
        => await http.ExecuteAsync(new Uri(url), cancellationToken);

    public static async ValueTask<string> ExecuteAsync(
        this HttpClient http,
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async ValueTask<string> ExecuteAsync(
        this HttpClient http,
        string url,
        Dictionary<string, string>? headers,
        CancellationToken cancellationToken = default)
        => await http.ExecuteAsync(new Uri(url), headers, cancellationToken);

    public static async ValueTask<string> ExecuteAsync(
        this HttpClient http,
        Uri uri,
        Dictionary<string, string>? headers,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        for (var i = 0; i < headers?.Count; i++)
        {
            request.Headers.TryAddWithoutValidation(
                headers.ElementAt(i).Key,
                headers.ElementAt(i).Value
            );
        }

        return await http.ExecuteAsync(request, cancellationToken);
    }

    public static async Task<string> ExecuteAsync(
        this HttpClient http,
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
            return string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})." +
                Environment.NewLine +
                "Request:" +
                Environment.NewLine +
                request
            );
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}