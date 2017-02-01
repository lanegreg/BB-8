using Car.Com.Domain.Models.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface IVehicleContentService
  {
    Task<Article> GetArticleByIdAsync(int contentId);
    Article GetArticleById(int contentId);

    Task<ICollection<string>> GetAllArticleUrlsForSitemapAsync();
    ICollection<string> GetAllArticleUrlsForSitemap();

    Task<ICollection<ArticlePage>> GetArticlePagesAsync(int contentId);
    ICollection<ArticlePage> GetArticlePages(int contentId);

    Task<ArticlePage> GetArticlePageAsync(int contentId, int page);
    ArticlePage GetArticlePage(int contentId, int page);

    Task<ICollection<ArticleAd>> GetArticleAdsAsync(int contentId, int page);
    ICollection<ArticleAd> GetArticleAds(int contentId, int page);

    Task<ArticleList> GetArticlesByVehiclesCategoryAsync();
    ArticleList GetArticlesByVehiclesCategory();

    Task<ArticleList> GetArticlesGroupMostRecentAsync();
    ArticleList GetArticlesGroupMostRecent();

    Task<ArticleListWithCategory> GetArticlesByCategoryAttributeSeoNameAsync(string seoName);
    ArticleListWithCategory GetArticlesByCategoryAttributeSeoName(string seoName);
    
    Task<ArticleList> GetArticlesByTopicAsync(string topic, int startRow, int take);
    ArticleList GetArticlesByTopic(string topic, int startRow, int take);

    Task<ArticleList> GetArticlesByMakeAsync(int makeId, int startRow, int take);
    ArticleList GetArticlesByMake(int makeId, int startRow, int take);

    Task<ArticleList> GetArticlesByMakeModelAsync(int makeId, int modelId, int startRow, int take);
    ArticleList GetArticlesByMakeModel(int makeId, int modelId, int startRow, int take);

    Task<ArticleList> GetArticlesByMakeModelYearAsync(int makeId, int modelId, int year, int startRow, int take);
    ArticleList GetArticlesByMakeModelYear(int makeId, int modelId, int year, int startRow, int take);

    Task<ArticleList> GetRelatedArticlesAsync(int contentId, int startRow, int take, bool fillWithLatest);
    ArticleList GetRelatedArticles(int contentId, int startRow, int take, bool fillWithLatest);

    Task<Author> GetAuthorByIdAsync(int authorId); 
    Author GetAuthorById(int authorId);

    Task<ICollection<Author>> GetContentAuthorsAsync(int contentId); 
    ICollection<Author> GetContentAuthors(int contentId);

    Task<ICollection<Author>> GetAllAuthorsAsync(); 
    ICollection<Author> GetAllAuthors();

  }
}