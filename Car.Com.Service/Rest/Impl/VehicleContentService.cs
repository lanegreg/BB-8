using Car.Com.Common;
using Car.Com.Common.Cache;
using Car.Com.Domain.Models.Content;
using Car.Com.Domain.Services;
using Car.Com.Service.Rest.Common;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Service.Rest.Impl
{
  public sealed class VehicleContentService : RestServiceBase<ServiceJsonEnvelope>, IVehicleContentService
  {

    #region Declarations

    private const int MinimumResearchYear = 2007;
    private const string VehicleContentTierOneHashKey = "vcs_svc:t1_cache";
    private const string VehicleContentTierTwoHashKey = "vcs_svc:t2_cache";

    private const string ArticleFieldName = "article";
    private const string ArticlesByTopicFieldName = "articlesbytopic";
    private const string AuthorFieldName = "author";
    private const string AuthorsFieldName = "authors";
    private const double LocalCacheTimeToLiveInSecs = 120; // 2 Minutes

    private const string ArticlesByCategory = "categoryarticlelist";

    private static readonly ConnectionMultiplexer RedisReadable;
    private static readonly Uri Endpoint;
    private static readonly string PathPrefix;

    private static readonly int DatabaseNumber = WebConfig.Get<int>("VehicleContentService:Redis:DatabaseNumber");

    #endregion


    #region ctors

    static VehicleContentService()
    {
      RedisReadable = ConnectionMultiplexer.Connect(WebConfig.Get<string>("Redis:Readable:Config"));
      PathPrefix = String.Format("/api/v{0}/", WebConfig.Get<string>("VehicleContentService:ApiVersion"));
      Endpoint = new Uri(String.Format("http://{0}", WebConfig.Get<string>("VehicleContentService:Endpoint")));
    }

    public VehicleContentService()
      : base(WebConfig.Get<int>("VehicleContentService:Timeout_ms"))
    { }

    #endregion


    #region Article Methods

    public async Task<Article> GetArticleByIdAsync(int contentId)
    {
      var cacheKey =
        String.Format("{0}:article:by_contentid[{1}]",
          VehicleContentTierTwoHashKey, contentId);

      // First, check LocalCache.
      var article = LocalCache.Get<Article>(cacheKey);
      if (article != null)
        return article;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/{0}", contentId);

        article = await FetchResource<Article>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, article);
        return article;
      }

      article = JsonConvert.DeserializeObject<Article>(json);
      LocalCache.Put(cacheKey, article);
      return article;
    }

    public Article GetArticleById(int contentId)
    {
      return GetArticleByIdAsync(contentId).Result;
    }

    public async Task<ICollection<string>> GetAllArticleUrlsForSitemapAsync()
    {
      var cacheKey =
        String.Format("{0}:article:all_article_urls_for_sitemap",
          VehicleContentTierTwoHashKey);

      // First, check LocalCache.
      var articleUrls = LocalCache.Get<ICollection<string>>(cacheKey);
      if (articleUrls != null)
        return articleUrls;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = "/articles/allarticleurlsforsitemap";

        articleUrls = await FetchResource<ICollection<string>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleUrls);
        return articleUrls;
      }

      articleUrls = JsonConvert.DeserializeObject<List<string>>(json);
      LocalCache.Put(cacheKey, articleUrls);
      return articleUrls;
    }

    public ICollection<string> GetAllArticleUrlsForSitemap()
    {
      return GetAllArticleUrlsForSitemapAsync().Result;
    }

    public async Task<ICollection<ArticlePage>> GetArticlePagesAsync(int contentId)
    {
      var cacheKey =
        String.Format("{0}:articlepages:by_contentid[{1}]",
          VehicleContentTierTwoHashKey, contentId);

      // First, check LocalCache.
      var articlePages = LocalCache.Get<ICollection<ArticlePage>>(cacheKey);
      if (articlePages != null)
        return articlePages;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/{0}", contentId);

        articlePages = await FetchResource<ICollection<ArticlePage>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articlePages);
        return articlePages;
      }

      articlePages = JsonConvert.DeserializeObject<List<ArticlePage>>(json);
      LocalCache.Put(cacheKey, articlePages);
      return articlePages;
    }

    public ICollection<ArticlePage> GetArticlePages(int contentId)
    {
      return GetArticlePagesAsync(contentId).Result;
    }
    

    public async Task<ArticlePage> GetArticlePageAsync(int contentId, int page)
    {
      var cacheKey =
        String.Format("{0}:articlepage:by_contentid[{1}]:by_page[{2}]",
          VehicleContentTierTwoHashKey, contentId, page);

      // First, check LocalCache.
      var articlePage = LocalCache.Get<ArticlePage>(cacheKey);
      if (articlePage != null)
        return articlePage;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/{0}/{1}", contentId, page);

        articlePage = await FetchResource<ArticlePage>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articlePage);
        return articlePage;
      }

      articlePage = JsonConvert.DeserializeObject<ArticlePage>(json);
      LocalCache.Put(cacheKey, articlePage);
      return articlePage;
    }

    public ArticlePage GetArticlePage(int contentId, int page)
    {
      return GetArticlePageAsync(contentId, page).Result;
    }
    

    public async Task<ICollection<ArticleAd>> GetArticleAdsAsync(int contentId, int page)
    {
      var cacheKey =
        String.Format("{0}:articleads:by_contentid[{1}]:by_page[{2}]",
          VehicleContentTierTwoHashKey, contentId, page);

      // First, check LocalCache.
      var articleAds = LocalCache.Get<ICollection<ArticleAd>>(cacheKey);
      if (articleAds != null)
        return articleAds;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/ads/{0}/{1}", contentId, page);

        articleAds = await FetchResource<ICollection<ArticleAd>>(GetResourceUri(Endpoint, PathPrefix, path))
        .ConfigureAwait(false);

        LocalCache.Put(cacheKey, articleAds);
        return articleAds;
      }

      articleAds = JsonConvert.DeserializeObject<List<ArticleAd>>(json);
      LocalCache.Put(cacheKey, articleAds);
      return articleAds;
    }

    public ICollection<ArticleAd> GetArticleAds(int contentId, int page)
    {
      return GetArticleAdsAsync(contentId, page).Result;
    }

    public async Task<ArticleList> GetArticlesGroupMostRecentAsync()
    {
      const string topic = "Most Recent";

      var cacheKey = String.Format("articlelistbygroup:by_topic[{0}]", topic);

      // First check LocalCache

      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = "/articles/topic/mostrecent/";

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;

    }

    public ArticleList GetArticlesGroupMostRecent()
    {
      return GetArticlesGroupMostRecentAsync().Result;
    }


    public async Task<ArticleList> GetArticlesByVehiclesCategoryAsync()
    {
      const string cacheKey = ArticlesByCategory;

      // First check LocalCache

      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = "/articles/articlesbyvehiclescategory/";

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetArticlesByVehiclesCategory()
    {
      return GetArticlesByVehiclesCategoryAsync().Result;
    }

    public async Task<ArticleListWithCategory> GetArticlesByCategoryAttributeSeoNameAsync(string seoName)
    {
      var cacheKey = String.Format("articlelist:for_category[{0}]", seoName);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleListWithCategory>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/categoryarticles/{0}", seoName);

        articleList = await FetchResource<ArticleListWithCategory>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleListWithCategory>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleListWithCategory GetArticlesByCategoryAttributeSeoName(string seoName)
    {
      return GetArticlesByCategoryAttributeSeoNameAsync(seoName).Result;
    }

    public async Task<ArticleList> GetArticlesByTopicAsync(string topic, int startRow, int take)
    {
      var cacheKey =
        String.Format("{0}:articlelist:by_topic[{1}]:by_startrow[{2}]:by_take[{3}]",
          VehicleContentTierTwoHashKey, topic, startRow, take);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/topic/{0}/?start={1}&take={2}", topic, startRow, take);

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetRelatedArticles(int contentId, int start, int take, bool fillWithLatest)
    {
      return GetRelatedArticlesAsync(contentId, start, take, fillWithLatest).Result;
    }

    public async Task<ArticleList> GetRelatedArticlesAsync(int contentId, int start, int take, bool fillWithLatest)
    {
      var cacheKey =
        String.Format("{0}:relatedarticles:by_contentid[{1}]:by_start[{2}]:by_take[{3}]:fill_with_latest[{4}]",
          VehicleContentTierTwoHashKey, contentId, start, take, fillWithLatest);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/related/{0}/?start={1}&take={2}&fillWithLatest={3}", contentId, start, take, fillWithLatest);

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetArticlesByTopic(string topic, int startRow, int take)
    {
      return GetArticlesByTopicAsync(topic, startRow, take).Result;
    }



    public async Task<ArticleList> GetArticlesByMakeAsync(int makeId, int startRow, int take)
    {
      var cacheKey =
        String.Format("{0}:articlelist:by_makeid[{1}]:by_startrow[{2}]:by_take[{3}]",
          VehicleContentTierTwoHashKey, makeId, startRow, take);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/make/{0}/?start={1}&take={2}", makeId, startRow, take);

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetArticlesByMake(int makeId, int startRow, int take)
    {
      return GetArticlesByMakeAsync(makeId, startRow, take).Result;
    }
    

    public async Task<ArticleList> GetArticlesByMakeModelAsync(int makeId, int modelId, int startRow, int take)
    {
      var cacheKey =
        String.Format("{0}:articlelist:by_makeid[{1}]:by_modelid[{2}]:by_startrow[{3}]:by_take[{4}]",
          VehicleContentTierTwoHashKey, makeId, modelId, startRow, take);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/make/{0}/model/{1}/?start={2}&take={3}", makeId, modelId, startRow, take);

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetArticlesByMakeModel(int makeId, int modelId, int startRow, int take)
    {
      return GetArticlesByMakeModelAsync(makeId, modelId, startRow, take).Result;
    }
    

    public async Task<ArticleList> GetArticlesByMakeModelYearAsync(int makeId, int modelId, int year, int startRow, int take)
    {
      var cacheKey =
        String.Format("{0}:articlelist:by_makeid[{1}]:by_modelid[{2}]:by_year[{3}]by_startrow[{4}]:by_take[{5}]",
          VehicleContentTierTwoHashKey, makeId, modelId, year, startRow, take);

      // First, check LocalCache.
      var articleList = LocalCache.Get<ArticleList>(cacheKey);
      if (articleList != null)
        return articleList;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/articles/make/{0}/model/{1}/{2}/?start={3}&take={4}", makeId, modelId, year, startRow, take);

        articleList = await FetchResource<ArticleList>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, articleList);
        return articleList;
      }

      articleList = JsonConvert.DeserializeObject<ArticleList>(json);
      LocalCache.Put(cacheKey, articleList);
      return articleList;
    }

    public ArticleList GetArticlesByMakeModelYear(int makeId, int modelId, int year, int startRow, int take)
    {
      return GetArticlesByMakeModelYearAsync(makeId, modelId, year, startRow, take).Result;
    }

    #endregion


    #region Author Methods

    public async Task<Author> GetAuthorByIdAsync(int authorId)
    {
      var cacheKey =
        String.Format("{0}:author:by_authorid[{1}]",
          VehicleContentTierTwoHashKey, authorId);

      // First, check LocalCache.
      var author = LocalCache.Get<Author>(cacheKey);
      if (author != null)
        return author;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/authors/{0}", authorId);

        author = await FetchResource<Author>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, author);
        return author;
      }

      author = JsonConvert.DeserializeObject<Author>(json);
      LocalCache.Put(cacheKey, author);
      return author;
    }

    public Author GetAuthorById(int authorId)
    {
      return GetAuthorByIdAsync(authorId).Result;
    }


    public async Task<ICollection<Author>> GetContentAuthorsAsync(int contentId)
    {
      var cacheKey =
        String.Format("{0}:authors:by_contentId[{1}]",
          VehicleContentTierTwoHashKey, contentId);

      // First, check LocalCache.
      var authors = LocalCache.Get<ICollection<Author>>(cacheKey);
      if (authors != null)
        return authors;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierTwoHashKey, cacheKey)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        var path = String.Format("/authors/article/{0}", contentId);

        authors = await FetchResource<ICollection<Author>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, authors);
        return authors;
      }

      authors = JsonConvert.DeserializeObject<List<Author>>(json);
      LocalCache.Put(cacheKey, authors);
      return authors;
    }

    public ICollection<Author> GetContentAuthors(int contentId)
    {
      return GetContentAuthorsAsync(contentId).Result;
    }


    public async Task<ICollection<Author>> GetAllAuthorsAsync()
    {
      const string cacheKey = AuthorsFieldName;

      // First, check LocalCache.
      var authors = LocalCache.Get<ICollection<Author>>(AuthorsFieldName);
      if (authors != null)
        return authors;

      //check redis
      var json = await RedisReadable
        .GetDatabase(DatabaseNumber)
        .HashGetAsync(VehicleContentTierOneHashKey, AuthorsFieldName)
        .ConfigureAwait(false);

      if (json.IsNullOrEmpty)
      {
        const string path = "/authors";

        authors = await FetchResource<ICollection<Author>>(GetResourceUri(Endpoint, PathPrefix, path))
          .ConfigureAwait(false);
        LocalCache.Put(cacheKey, authors);
        return authors;
      }

      authors = JsonConvert.DeserializeObject<List<Author>>(json);
      LocalCache.Put(cacheKey, authors);
      return authors;
    }

    public ICollection<Author> GetAllAuthors()
    {
      return GetAllAuthorsAsync().Result;
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
