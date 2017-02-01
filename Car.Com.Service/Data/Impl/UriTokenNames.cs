using System.Collections.Generic;

namespace Car.Com.Service.Data.Impl
{
  public sealed class UriTokenNames
  {
    public const string Id = "id";
    public const string Letter = "letter";
    public const string Make  = "makeSeoName";
    public const string Year = "yearNumber";
    public const string Trim = "trimSeoName";
    public const string Category = "categorySeoName";
    public const string City = "citySeoName";
    public const string State = "stateSeoName";
    public const string Zip = "zipNumber";
    public const string Title = "titleSeoName";
    public const string SuperModel = "superModelSeoName";
    public const string VehicleAttribute = "vehicleAttributeSeoName";
    public const string AltFuel = "altFuelSeoName";
    public const string Page = "page";
    public const string ContentId = "contentId";
    public const string ArticleTitle = "title";
    

    public static IList<TokenMap> All = new List<TokenMap>
    {
      new TokenMap {ParamName = Make, TokenName = "{make}"},
      new TokenMap {ParamName = SuperModel, TokenName = "{super-model}"},
      new TokenMap {ParamName = Year, TokenName = "{year}"},
      new TokenMap {ParamName = Trim, TokenName = "{trim}"},
      new TokenMap {ParamName = Category, TokenName = "{category}"},
      new TokenMap {ParamName = City, TokenName = "{city}"},
      new TokenMap {ParamName = State, TokenName = "{state}"},
      new TokenMap {ParamName = Zip, TokenName = "{zip}"},
      new TokenMap {ParamName = Title, TokenName = "{title}"},
      new TokenMap {ParamName = VehicleAttribute, TokenName = "{vehicle-attrib}"},
      new TokenMap {ParamName = AltFuel, TokenName = "{altfuel}"},
      new TokenMap {ParamName = Page, TokenName = "{page}"},
      new TokenMap {ParamName = ContentId, TokenName = "{contentid}"},
      new TokenMap {ParamName = ArticleTitle, TokenName = "{articletitle}"}
    };

    public class TokenMap
    {
      public string ParamName { get; set; }
      public string TokenName { get; set; }
    }
  }
}
