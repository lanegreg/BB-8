using Car.Com.Common;
using Car.Com.Domain.Models.SiteMeta;
using Car.Com.Domain.Services;
using Car.Com.Service.Common;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Car.Com.Service.Data.Impl
{
  public sealed class MetadataService : IMetadataService, ICacheable
  {
    #region Declarations

    private static readonly Regex TokenRegex = new Regex(@"\{[a-z\-]+\}?", RegexOptions.Compiled);
    private static ICollection<PageTemplate> _pageTemplates;

    #endregion


    #region Interface Impls

    public void Warm()
    {
      using (var conn = VehicleContentDbConn())
      {
        _pageTemplates = (ICollection<PageTemplate>)conn.Query<PageTemplate>("CCWeb.GetPageMetaCache");
      }
    }

    public IMetadata GetMetadataForPage(HttpContextBase httpContext)
    {
      return GetMetadataForPage(httpContext, null);
    }

    public IMetadata GetMetadataForPage(HttpContextBase httpContext, IDictionary<string, string> additionalTokenMaps)
    {
      const string domain = "http://www.car.com";

      // ReSharper disable once PossibleNullReferenceException
      var urlPath = httpContext.Request.Url.AbsolutePath;
      //var hostName = httpContext.Request.Url.DnsSafeHost;


      // Clean-up and combine all token-maps
      var tokenMaps = httpContext.Request.RequestContext.RouteData.Values
        .Where(v => !(new[] {"controller", "action"}).Contains(v.Key))
        .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

      if (additionalTokenMaps != null)
        tokenMaps = (Dictionary<string, string>) tokenMaps.Union(additionalTokenMaps);

      var pageMeta = GetPageMetaForUrl(urlPath, tokenMaps);
      pageMeta.Canonical = domain + pageMeta.Canonical;

      return new Metadata
      {
        PageMeta = pageMeta
      };
    }
    
    public PageMeta GetPageMetaForUrl(string urlPath, IDictionary<string, string> tokenMaps)
    {
      // We need to query the templates for a match to this request url
      var templateForThisPage = GetTemplateForThisPage(urlPath, tokenMaps);

      // If no template was matched, return default values
      if (templateForThisPage == null)
      {
        return new PageMeta
        {
          Canonical = urlPath.EnsureTrailingForwardSlash(),
          Description = String.Empty,
          Title = String.Empty,
          H1 = String.Empty,
          H2 = String.Empty,
          LinkbackAnchorText = String.Empty,
          LinkbackAnchorTitle = String.Empty,
          Keywords = String.Empty,
          Breadcrumbs = new List<Breadcrumb>(),
          SiteSection = String.Empty,
          ContentSection = String.Empty,
          SubSection = String.Empty,
          PageName = String.Empty
        };
      }


      // Get an un-mapped version of the metadata
      var unMappedMetadata = JsonConvert.DeserializeObject<Metadata>(templateForThisPage.Metadata);
      // Get a display-mapped version of the metadata
      var displayMappedMetadata = GetDisplayMappedMetadata(templateForThisPage, tokenMaps);


      return new PageMeta
      {
        WasMappedWithSeoMetadata = true,
        Canonical = GetCanonical(displayMappedMetadata, urlPath),
        Description = displayMappedMetadata.MetaDescription,
        Title = displayMappedMetadata.HtmlTitle,
        H1 = displayMappedMetadata.H1,
        H2 = displayMappedMetadata.H2,
        LinkbackAnchorText = displayMappedMetadata.LinkBackAnchorText,
        LinkbackAnchorTitle = displayMappedMetadata.LinkBackTitleAttr,
        Keywords = GetStringifiedKeywords(displayMappedMetadata),
        Breadcrumbs = GetBreadcrumbsMappedToMetadata(unMappedMetadata, tokenMaps, displayMappedMetadata.LinkBackBreadcrumbText),
        SiteSection = displayMappedMetadata.SiteSection,
        ContentSection = displayMappedMetadata.ContentSection,
        SubSection = displayMappedMetadata.SubSection,
        PageName = displayMappedMetadata.PageName
      };
    }

    #endregion


    #region Static Methods

    public static string ConvertToUrlPathPattern(string urlPath, IEnumerable<KeyValuePair<string, string>> tokenMaps)
    {
      // Token Ex: {key: "makeSeoName", value: "aston-martin"}
      foreach (var map in tokenMaps)
      {
        var value = String.Format("/{0}/", map.Value.ToString(CultureInfo.InvariantCulture));

        switch (map.Key)
        {
          case UriTokenNames.Make:
            urlPath = urlPath.Replace(value, "/{make}/");
            break;

          case UriTokenNames.SuperModel:
            // This regex code replaces only the first one. Fixes issue where supermodel and trim have same value 
            // (ex: supermodel...shelby-gt350, trim...shelby-gt350), and the old replace was replacing them both at the same time and causing an error.
            var regex = new Regex(Regex.Escape(value));
            urlPath = regex.Replace(urlPath, "/{super-model}/", 1);
            break;

          case UriTokenNames.Year:
            urlPath = urlPath.Replace(value, "/{year}/");
            break;

          case UriTokenNames.Trim:
            urlPath = urlPath.Replace(value, "/{trim}/");
            break;

          case UriTokenNames.Category:
            urlPath = urlPath.Replace(value, "/{category}/");
            break;

          case UriTokenNames.VehicleAttribute:
            urlPath = urlPath.Replace(value, "/{vehicle-attrib}/");
            break;

          case UriTokenNames.Page:
            urlPath = urlPath.Replace(value, "/{page}/");
            break;

          case UriTokenNames.ContentId:
            value = value.TrimStart('/');
            urlPath = urlPath.Replace(value, "{contentid}/");
            break;

          case UriTokenNames.ArticleTitle:
            value = value.TrimEnd('/');
            urlPath = urlPath.Replace(value, "/{articletitle}");
            break;

        }
      }

      return urlPath.EnsureTrailingForwardSlash();
    }


    public static string MapMetadataTokensWithSeoValues(string metadata, IEnumerable<KeyValuePair<string, string>> tokenMaps)
    {
      // Token Ex: {key: "makeSeoName", value: "aston-martin"}
      foreach (var map in tokenMaps)
      {
        var value = map.Value.ToString(CultureInfo.InvariantCulture);

        switch (map.Key)
        {
          case UriTokenNames.Make:
            metadata = metadata.Replace("{make}", value);
            break;

          case UriTokenNames.SuperModel:
            // This regex code replaces only the first one. Fixes issue where supermodel and trim have same value 
            // (ex: supermodel...shelby-gt350, trim...shelby-gt350), and the old replace was replacing them both at the same time and causing error.
            var regex = new Regex(Regex.Escape(value));
            metadata = regex.Replace(metadata, "{super-model}", 1);
            break;

          case UriTokenNames.Year:
            metadata = metadata.Replace("{year}", value);
            break;

          case UriTokenNames.Trim:
            metadata = metadata.Replace("{trim}", value);
            break;

          case UriTokenNames.Category:
            metadata = metadata.Replace("{category}", value);
            break;

          case UriTokenNames.ArticleTitle:
            metadata = metadata.Replace("{articletitle}", value);
            break;

          case UriTokenNames.ContentId:
            metadata = metadata.Replace("{contentid}", value);
            break;
        }
      }

      return metadata;
    }


    public static string MapMetadataTokensWithDisplayValues(string metadata, IEnumerable<KeyValuePair<string, string>> tokenMaps)
    {
      // Token Ex: {key: "makeSeoName", value: "aston-martin"}
      foreach (var map in tokenMaps)
      {
        var value = map.Value.ToString(CultureInfo.InvariantCulture);

        switch (map.Key)
        {
          case UriTokenNames.Make:
            var makeTrans = UriTokenTranslators.GetMakeTranslatorBySeoName(value);
            metadata = metadata.Replace("{make}s", makeTrans.PluralName);
            metadata = metadata.Replace("{make}", makeTrans.Name);
            break;

          case UriTokenNames.SuperModel:
            var superModelTrans = UriTokenTranslators.GetSuperModelTranslatorBySeoName(value);
            metadata = metadata.Replace("{super-model}s", superModelTrans.Name + "s");
            metadata = metadata.Replace("{super-model}", superModelTrans.Name);
            break;

          case UriTokenNames.Year:
            metadata = metadata.Replace("{year}", value);
            break;

          case UriTokenNames.Trim:
            var trimTrans = UriTokenTranslators.GetTrimTranslatorBySeoName(value);
            metadata = metadata.Replace("{trim}", trimTrans.Name);
            break;

          case UriTokenNames.Category:
            var categoryTrans = UriTokenTranslators.GetCategoryTranslatorBySeoName(value);
            metadata = metadata.Replace("{category}", categoryTrans.PluralName);
            metadata = metadata.Replace("{category-singular}", categoryTrans.Name);
            break;

          case UriTokenNames.VehicleAttribute:
            var vehicleAttributeTrans = UriTokenTranslators.GetVehicleAttributeTranslatorBySeoName(value);
            metadata = metadata.Replace("{vehicle-attrib}", vehicleAttributeTrans.Name);
            break;

          case UriTokenNames.ArticleTitle:
            metadata = metadata.Replace("{articletitle}", value);
            break;

          case UriTokenNames.ContentId:
            metadata = metadata.Replace("{contentid}", value);
            break;
        }
      }

      return metadata;
    }


    public static PageTemplate GetTemplateForThisPage(string urlPath, ICollection<KeyValuePair<string, string>> tokenMaps)
    {
      var urlPathPatternToMatch = urlPath.EnsureTrailingForwardSlash();

      // Quick check to see if we get a match as-is, without mapping anything.
      var template = _pageTemplates.FirstOrDefault(t => t.UrlPathPattern.Equals(urlPathPatternToMatch));
      if (template != null)
        return template;


      // Now, we are going to work from right to left, unmapping the right most token first, then checking for a template match.
      // If we have no match, we unmap the next token to the left, and then check again.
      var tokenNames = TokenRegex
        .Matches(ConvertToUrlPathPattern(urlPathPatternToMatch, tokenMaps))
        .Cast<Match>()
        .Reverse()
        .Select(m => m.Value)
        .ToList();


      #region -- This code is isolating a specific scenario where SuperModel name is equal to the Trim name.
      const string superModelKeyName = "superModelSeoName";
      const string trimKeyName = "trimSeoName";

      var weHaveBothSuperModelAndTrimTokens =
        tokenMaps.Any(t => t.Key == superModelKeyName)
        && tokenMaps.Any(t => t.Key == trimKeyName);

      if (weHaveBothSuperModelAndTrimTokens)
      {
        var superModelSeoNameValue = "superModelSeoNameValue";
        var trimSeoNameValue = "trimSeoNameValue";
        var matchFirst = false;
        
        foreach (var keyval in tokenMaps)
        {
          switch (keyval.Value)
          {
            case superModelKeyName:
              superModelSeoNameValue = keyval.Value;
              break;

            case trimKeyName:
              trimSeoNameValue = keyval.Value;
              break;
          }
        }

        //If our supermodel and trim seo names are equal we need to un-reverse the tokenNames and work left to right.
        //If we go right to left we update supermodel and trim in the wrong order where they have the same value.
        if (superModelSeoNameValue == trimSeoNameValue)
        {
          tokenNames = TokenRegex
          .Matches(ConvertToUrlPathPattern(urlPathPatternToMatch, tokenMaps))
          .Cast<Match>()
          .Select(m => m.Value)
          .ToList();

          matchFirst = true;
        }

        foreach (var tokenName in tokenNames)
        {
          var tokenMap = UriTokenNames.All.First(m => m.TokenName == tokenName);
          var map = tokenMaps
            .Where(t => t.Key == tokenMap.ParamName)
            .Select(m => new
            {
              tokenMap.ParamName,
              tokenMap.TokenName,
              m.Value
            })
            .Single();

          // This regex code replaces only the first one. Fixes issue where supermodel and trim have same value 
          // and the old replace was replacing them both at the same time and causing an error.
          var regex = new Regex(Regex.Escape("/" + map.Value + "/"));

          if (!matchFirst)
          {
            //Reverse and replace only the last one. Fixes issue where supermodel and trim have same value 
            //and the old replace was replacing them both at the same time and causing an error.
            regex = new Regex(Regex.Escape("/" + map.Value + "/"), RegexOptions.RightToLeft);
          }

          urlPathPatternToMatch = regex.Replace(urlPathPatternToMatch, "/" + map.TokenName + "/", 1);
          template = _pageTemplates.FirstOrDefault(t => t.UrlPathPattern.Equals(urlPathPatternToMatch));

          if (template != null)
            return template;
        }
      }
      
      #endregion


      foreach (var tokenName in tokenNames)
      {
        var tokenMap = UriTokenNames.All.First(m => m.TokenName == tokenName);
        var map = tokenMaps
          .Where(t => t.Key == tokenMap.ParamName)
          .Select(m => new
          {
            tokenMap.ParamName,
            tokenMap.TokenName,
            m.Value
          })
          .Single();

        urlPathPatternToMatch = urlPathPatternToMatch.Replace(map.Value, map.TokenName);
        template = _pageTemplates.FirstOrDefault(t => t.UrlPathPattern.Equals(urlPathPatternToMatch));

        if (template != null)
          return template;
      }
      
      return null;
    }


    public static Metadata GetDisplayMappedMetadata(PageTemplate template, IEnumerable<KeyValuePair<string, string>> tokenMaps)
    {
      
      var metadataWithDisplayValues = MapMetadataTokensWithDisplayValues(template.Metadata, tokenMaps);
      return JsonConvert.DeserializeObject<Metadata>(metadataWithDisplayValues);
    }


    private static string GetStringifiedKeywords(Metadata metadata)
    {
      return String.Join(",",
        metadata.Keywords
          .OrderBy(k => k.OrdinalPosition)
          .Select(k => k.Text));
    }


    private static string GetCanonical(Metadata metadata, string urlPath)
    {
      // Use the seo canonical, or fallback to the actual url of the page
      var canonical = metadata.Canonical.IsNotNullOrEmpty()
        ? metadata.Canonical
        : urlPath;

      return canonical.EnsureTrailingForwardSlash();
    }


    private static IEnumerable<Breadcrumb> GetBreadcrumbsMappedToMetadata
      (Metadata metadata, ICollection<KeyValuePair<string, string>> tokenMaps, string textForCurrentPage)
    {
      if (metadata.Breadcrumbs.Any())
      {
        // Map breadcrumb properties to their real values
        var lastOrdinal = metadata.Breadcrumbs.Last().OrdinalPosition;

        foreach (var crumb in metadata.Breadcrumbs)
        {
          if (crumb.OrdinalPosition != lastOrdinal)
          {
            // ReSharper disable once AccessToForEachVariableInClosure
            var pageTemplate = _pageTemplates.First(t => t.UrlPathPattern.Equals(crumb.UriPattern));

            // Get a display-mapped version of the metadata
            var breadcrumbsWithDisplayValues = MapMetadataTokensWithDisplayValues(pageTemplate.Metadata, tokenMaps);
            var breadcrumbsDeserializedWithDisplayValues = JsonConvert.DeserializeObject<Metadata>(breadcrumbsWithDisplayValues);
            // Get an seo-mapped version of the metadata
            var breadcrumbsWithSeoValues = MapMetadataTokensWithSeoValues(pageTemplate.Metadata, tokenMaps);
            var breadcrumbsDeserializedWithSeoValues = JsonConvert.DeserializeObject<Metadata>(breadcrumbsWithSeoValues);

            crumb.IsAnchor = true;
            crumb.Href = breadcrumbsDeserializedWithSeoValues.UriPattern;
            crumb.Text = breadcrumbsDeserializedWithDisplayValues.LinkBackBreadcrumbText;
            crumb.Title = breadcrumbsDeserializedWithDisplayValues.LinkBackTitleAttr;

            continue;
          }

          crumb.Text = textForCurrentPage;
        }
      }

      return metadata.Breadcrumbs;
    }

    #endregion


    #region Connections

    private static readonly string VehicleContentConnString = WebConfig.Get<string>("ConnectionString:VehicleContent");
    private static IDbConnection VehicleContentDbConn()
    {
      return ServiceLocator.Get<IDbConnectionFactory>().CreateConnection(VehicleContentConnString);
    }

    #endregion
  }
}
