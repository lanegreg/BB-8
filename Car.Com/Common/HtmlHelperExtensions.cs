using Car.Com.Domain.Services;
using Car.Com.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Car.Com.Common
{
  public static class HtmlHelperExtensions
  {
    #region Declarations

    private const string AnchorTextPlaceholder = "{SEO_ANCHOR_TEXT}";
    private const string AnchorTitlePlaceholder = "{SEO_ANCHOR_TITLE}";
    private static readonly Regex HrefRegex = new Regex("href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))", RegexOptions.Compiled);

    #endregion

    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName)
    {
      return SeoRouteLink(htmlHelper, routeName, false);
    }

    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName, bool disableSeo)
    {
      return SeoRouteLink(htmlHelper, routeName, null, null, null, disableSeo);
    }


    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName, RouteValues routeValues)
    {
      return SeoRouteLink(htmlHelper, routeName, routeValues, false);
    }

    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName, RouteValues routeValues, bool disableSeo)
    {
      return SeoRouteLink(htmlHelper, routeName, routeValues, null, null, disableSeo);
    }


    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName, RouteValues routeValues, string linkText)
    {
      return SeoRouteLink(htmlHelper, routeName, routeValues, linkText, false);
    }

    public static MvcHtmlString SeoRouteLink(this HtmlHelper htmlHelper, string routeName, RouteValues routeValues, string linkText, bool disableSeo)
    {
      return SeoRouteLink(htmlHelper, routeName, routeValues, linkText, null, disableSeo);
    }


    public static MvcHtmlString SeoRouteLink
      (this HtmlHelper htmlHelper, string routeName, RouteValues routeValues, string linkText, Attributes attributes)
    {
      return SeoRouteLink(htmlHelper, routeName, routeValues, linkText, attributes, false);
    }

    public static MvcHtmlString SeoRouteLink
      (this HtmlHelper htmlHelper, string routeName, RouteValues routeValues, string linkText, Attributes attributes, bool disableSeo)
    {
      routeValues = routeValues ?? new RouteValues();
      attributes = attributes ?? new Attributes();


      // This section of code handles the *disableSeo = true* scenario
      if (disableSeo)
      {
        if (!attributes.ContainsKey("title"))
          attributes.Add("title", linkText);

        return new MvcHtmlString(HttpUtility.HtmlDecode(htmlHelper.RouteLink(linkText, routeName, routeValues, attributes).ToHtmlString()));
      }



      if (!attributes.ContainsKey("title"))
        attributes.Add("title", AnchorTitlePlaceholder);

      var routeLink = htmlHelper.RouteLink(AnchorTextPlaceholder, routeName, routeValues, attributes).ToString();
      var match = HrefRegex.Match(routeLink);

      if (!match.Success)
        throw new Exception(String.Format(
          "HtmlHelperExtensions.SeoRouteLink(): Generated href from RouteName: '{0}' was empty. Possible RouteName or RouteValues problem.",
          routeName));

      var href = match.Value;
      var urlPath = href
        .Replace("href=", "")
        .Replace("\"", "")
        .EnsureTrailingForwardSlash();


      // Clean-up and combine all token-maps
      var tokenMaps = routeValues.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

      var metadataService = ServiceLocator.Get<IMetadataService>();
      var pageMeta = metadataService.GetPageMetaForUrl(urlPath, tokenMaps);

      if (!pageMeta.WasMappedWithSeoMetadata)
        throw new Exception(String.Format(
          "HtmlHelperExtensions.SeoRouteLink(): Could not find PageTemplateMetadata for href: '{0}'. ",
          urlPath));


      // Everything looks OK, so let's wrap it up here by replacing placeholders with values
      var seoRouteLink = routeLink
        .Replace(AnchorTextPlaceholder, (linkText ?? AnchorTextPlaceholder))
        .Replace(AnchorTextPlaceholder, pageMeta.LinkbackAnchorText)
        .Replace(AnchorTitlePlaceholder, pageMeta.LinkbackAnchorTitle)
        .Replace(href, "href=\"" + urlPath + "\"");

      return new MvcHtmlString(seoRouteLink);
    }
  }

  public class Attributes : Dictionary<string, object>
  {
  }

  public class RouteValues : RouteValueDictionary
  {
  }
}