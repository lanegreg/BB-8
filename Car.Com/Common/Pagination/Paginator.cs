using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Car.Com.Common.Pagination
{
  public class Paginator
  {
    private readonly IList<IPaginationItem> _paginationItems = new List<IPaginationItem>();
    private readonly string _prevTag;
    private readonly string _nextTag;


    public Paginator(string relativeUrl, int totalNumOfPages, int currentPageNum)
      : this(relativeUrl, totalNumOfPages, currentPageNum, 2)
    { }

    public Paginator(string relativeUrl, int totalNumOfPages, int currentPageNum, int numOfAdjacentLinksToDisplay)
    {
      var leftShowableLink = currentPageNum - numOfAdjacentLinksToDisplay;
      var rightShowableLink = currentPageNum + numOfAdjacentLinksToDisplay;

      // Possible 'Previous' link.
      if (currentPageNum > 1)
        _paginationItems.Add(new PreviousLink(relativeUrl, (currentPageNum - 1)));

      // Always add first page link.
      _paginationItems.Add(new PageLink(relativeUrl, 1, (currentPageNum == 1)));

      // Possible left-side ellipse.
      if ((leftShowableLink - 1) > 1)
        _paginationItems.Add(new Ellipse());

      // Add displayable links.
      for (var pageNum = leftShowableLink < 2 ? 2 : leftShowableLink;
           pageNum <= rightShowableLink && pageNum < totalNumOfPages;
           pageNum++)
        _paginationItems.Add(new PageLink(relativeUrl, pageNum, (currentPageNum == pageNum)));

      // Possible right-side ellipse.
      if ((totalNumOfPages - rightShowableLink) > 1)
        _paginationItems.Add(new Ellipse());

      // Always add last page link.
      if (totalNumOfPages > 1)
        _paginationItems.Add(new PageLink(relativeUrl, totalNumOfPages, (currentPageNum == totalNumOfPages)));

      // Possible 'Next' link.
      if (currentPageNum < totalNumOfPages)
        _paginationItems.Add(new NextLink(relativeUrl, (currentPageNum + 1)));



      // Head pagination *rel* tags.
      _prevTag = currentPageNum != 1
        ? String.Format("<link rel=\"prev\" href=\"{0}\" />",
          PaginationItem.MakeUrlPath(relativeUrl, (currentPageNum - 1)))
        : String.Empty;

      _nextTag = currentPageNum != totalNumOfPages
        ? String.Format("<link rel=\"next\" href=\"{0}\" />",
          PaginationItem.MakeUrlPath(relativeUrl, (currentPageNum + 1)))
        : String.Empty;
    }



    public HtmlString EmmitHtml()
    {
      var htmlFrag = new StringBuilder();
      foreach (var item in _paginationItems)
        htmlFrag.Append(item.Html);

      return new HtmlString(htmlFrag.ToString());
    }



    public HtmlString PrevTag()
    {
      return new HtmlString(_prevTag);
    }



    public HtmlString NextTag()
    {
      return new HtmlString(_nextTag);
    }



    public class PageLink : PaginationItem
    {
      private readonly bool _isActive;

      public PageLink(string relativeUrl, int pageNum, bool isActive)
        : base(relativeUrl, pageNum)
      {
        _isActive = isActive;
      }

      public override string Html
      {
        get
        {
          return _isActive
                   ? String.Format("<li class=\"active\"><span title=\"Page {0}\">{0}</span></li>", PageNum)
                   : String.Format("<li><a href=\"{0}\" title=\"Page {1}\">{1}</a></li>", Href, PageNum);
        }
      }
    }



    public class Ellipse : PaginationItem
    {
      public Ellipse()
        : base(String.Empty, 0)
      { }

      public override string Html
      {
        get
        {
          return "<li class=\"disabled\"><span>...</span></li>";
        }
      }
    }



    public class PreviousLink : PaginationItem
    {
      public PreviousLink(string relativeUrl, int pageNum)
        : base(relativeUrl, pageNum)
      { }

      public override string Html
      {
        get
        {
          return String.Format("<li><a href=\"{0}\"><span class=\"icon inline\"><svg><use xlink:href=\"/assets/svg/global-sprite.svg#i_arrow3_l\"></use></svg></span> Prev</a></li>", Href);
        }
      }
    }



    public class NextLink : PaginationItem
    {
      public NextLink(string relativeUrl, int pageNum)
        : base(relativeUrl, pageNum)
      { }

      public override string Html
      {
        get
        {
          return String.Format("<li><a href=\"{0}\">Next <span class=\"icon inline\"><svg><use xlink:href=\"/assets/svg/global-sprite.svg#i_arrow3_r\"></use></svg></span></a></li>", Href);
        }
      }
    }



    public abstract class PaginationItem : IPaginationItem
    {
      protected string Href;
      protected int PageNum;

      protected PaginationItem(string relativeUrl, int pageNum)
      {
        Href = MakeUrlPath(relativeUrl, pageNum);
        PageNum = pageNum;
      }

      public abstract string Html { get; }

      public static string MakeUrlPath(string relativeUrl, int pageNum)
      {
        return pageNum > 1
                 ? String.Format("{0}/{1}/", relativeUrl.TrimEnd('/'), pageNum)
                 : String.Concat(relativeUrl.TrimEnd('/'), "/");
      }
    }



    interface IPaginationItem
    {
      string Html { get; }
    }
  }
}