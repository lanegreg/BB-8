using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Image;
using Car.Com.Domain.Services;
using Car.Com.Service.Rest.Common;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Service.Rest.Impl
{
  public sealed class ImageMetaService : RestServiceBase<ServiceJsonEnvelope>, IImageMetaService
  {
    #region Declarations

    private const string ImageMetaTierOneHashKey = "img_svc:t1_cache";
    private const string ImageMetaTierTwoHashKey = "img_svc:t2_cache";
    private const string TierOneImageMetaByTrimIdFieldNameTempl = "images:by_trim_id[{0}]";
    private const string TierTwoImageMetaByTrimIdsFieldNameTempl = "images:by_trim_ids[{0}]";
    private const string TierTwoImageMetaByMakeByModelByYearFieldNameTempl = "images:by_make[{0}]:by_model[{1}]:by_year[{2}]";

    private static readonly ConnectionMultiplexer RedisReadable;
    private static readonly Uri Endpoint;
    private static readonly string PathPrefix;

    private static readonly int DatabaseNumber = WebConfig.Get<int>("ImageMetaService:Redis:DatabaseNumber");

    #endregion

    
    #region ctors

    static ImageMetaService()
    {
      RedisReadable = ConnectionMultiplexer.Connect(WebConfig.Get<string>("Redis:Readable:Config"));
      PathPrefix = String.Format("/api/v{0}/image-meta", WebConfig.Get<string>("ImageMetaService:ApiVersion"));
      Endpoint = new Uri(String.Format("http://{0}", WebConfig.Get<string>("ImageMetaService:Endpoint")));
    }

    public ImageMetaService()
      : base(WebConfig.Get<int>("ImageMetaService:Timeout_ms"))
    {}

    #endregion


    #region Interface Implemntation

    public async Task<IEnumerable<IImageMeta>> GetImagesByTrimIdsAsync(List<int> trimIds)
    {
      var pipeDelimitedTrimIds = String.Join("|", trimIds);

      // First, check LocalCache.
      var cacheKey = String.Format(TierTwoImageMetaByTrimIdsFieldNameTempl, pipeDelimitedTrimIds);
      var images = LocalCache.Get<ICollection<ImageMeta>>(cacheKey);
      if (images != null)
        return images;

      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(ImageMetaTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = "/images?trim_id=" + String.Join("&trim_id=", trimIds);

        return await FetchResource<List<ImageMeta>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
      }


      return JsonConvert.DeserializeObject<List<ImageMeta>>(json);
    }

    public IEnumerable<IImageMeta> GetImagesByTrimIds(List<int> trimIds)
    {
      return GetImagesByTrimIdsAsync(trimIds).Result;
    }



    public async Task<IEnumerable<IImageMeta>> GetImagesByTrimIdAsync(int trimId)
    {
      // First, check LocalCache.
      var localCacheKey = String.Format(TierOneImageMetaByTrimIdFieldNameTempl, trimId);
      var imageMeta = LocalCache.Get<ICollection<ImageMeta>>(localCacheKey);
      if (imageMeta != null)
        return imageMeta;


      // Then, check Tier 2 Redis.
      var redisCacheKey = String.Format("{0}:{1}", ImageMetaTierTwoHashKey, localCacheKey);
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(redisCacheKey)
        .ConfigureAwait(false);


      // Last, check the database directly.
      if (json.IsNullOrEmpty)
      {
        var path = "/images?trim_id=" + trimId;

        return await FetchResource<List<ImageMeta>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
      }


      return JsonConvert.DeserializeObject<List<ImageMeta>>(json);
    }

    public IEnumerable<IImageMeta> GetImagesByTrimId(int trimId)
    {
      return GetImagesByTrimIdAsync(trimId).Result;
    }



    public async Task<IEnumerable<IImageMeta>> GetImagesByMakeByModelByYearAsync(string makeSeoName, string modelSeoName, int yearNumber)
    {
      // First, check LocalCache.
      var localCacheKey =
        String.Format(TierTwoImageMetaByMakeByModelByYearFieldNameTempl,
          makeSeoName, modelSeoName, yearNumber);

      var imageMeta = LocalCache.Get<ICollection<ImageMeta>>(localCacheKey);
      if (imageMeta != null)
        return imageMeta;


      // Then, check Tier 2 Redis.
      var redisCacheKey = String.Format("{0}:{1}", ImageMetaTierTwoHashKey, localCacheKey);
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .StringGetAsync(redisCacheKey)
        .ConfigureAwait(false);


      // Last, check the database directly.
      if (json.IsNullOrEmpty)
      {
        var path =
          String.Format("/make/{0}/model/{1}/year/{2}/images",
            makeSeoName, modelSeoName, yearNumber);

        return await FetchResource<List<ImageMeta>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
      }


      return JsonConvert.DeserializeObject<List<ImageMeta>>(json);
    }

    public IEnumerable<IImageMeta> GetImagesByMakeByModelByYear(string makeSeoName, string modelSeoName, int yearNumber)
    {
      return GetImagesByMakeByModelByYearAsync(makeSeoName, modelSeoName, yearNumber).Result;
    }

    #endregion



    #region Local Memory Cache

    private static ILocalCache LocalCache
    {
      get { return ServiceLocator.Get<ILocalCache>(); }
    }

    #endregion
  }
}
