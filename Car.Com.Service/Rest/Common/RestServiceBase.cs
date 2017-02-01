using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Car.Com.Service.Rest.Common
{
  public abstract class RestServiceBase<TWrap> : IDisposable where TWrap : IWrapper
  {
    private readonly Lazy<HttpClient> _httpClient;


    protected RestServiceBase(int timeout)
    {
      _httpClient = new Lazy<HttpClient>(() => new HttpClient
      {
        Timeout = TimeSpan.FromMilliseconds(timeout)
      });
    }

    public void Dispose()
    {
      _httpClient.Value.Dispose();
    }

    #region Protected Methods

    protected HttpClient HttpClient
    {
      get { return _httpClient.Value; }
    }

    protected async Task<T> FetchResource<T>(Uri uri)
    {
      var json = await HttpClient.GetStringAsync(uri).ConfigureAwait(false);
      var wrapper = JsonConvert.DeserializeObject<TWrap>(json);

      var jsonString = wrapper.Data.ToString();
      var results = JsonConvert.DeserializeObject<T>(jsonString);

      return results;
    }

    protected static Uri GetResourceUri(Uri endpoint, string pathPrefix, string resourcePath)
    {
      var trimChar = new[] {'/'};
      return new Uri(endpoint, pathPrefix.TrimEnd(trimChar) + "/" + resourcePath.TrimStart(trimChar));
    }

    #endregion
  }
}
