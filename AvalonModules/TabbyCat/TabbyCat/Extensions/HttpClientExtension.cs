using System.Net.Http;
using TuDog.Extensions;

namespace TabbyCat.Extensions;

public static class HttpClientExtension
{
    public static Task<TResult?> PostAsync<TParams, TResult>(this HttpClient client, string url, string token,
        TParams request) where TParams : class
    {
        try
        {
            return client.PostRequestAsync<TParams, TResult>(url, request);
        }
        catch
        {
            return Task.FromResult(default(TResult));
        }
    }
}