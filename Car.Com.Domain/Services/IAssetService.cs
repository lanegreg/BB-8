using System.Web;

namespace Car.Com.Domain.Services
{
  public interface IAssetService
  {
    HtmlString GetInlineHeadStyles(string assetsPrefix);
    HtmlString GetInlineHeadScript();
  }
}
