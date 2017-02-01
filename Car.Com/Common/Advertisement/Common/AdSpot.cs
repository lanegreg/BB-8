using Newtonsoft.Json;
using System;

namespace Car.Com.Common.Advertisement.Common
{
  public class AdSpot
  {
    private const string AdServerPrefix = "http://ad.doubleclick.net/N2909/adj/";

    public AdSpot()
    {
      FirstLevel = Device = SecondLevel = ThirdLevel = Section = SubSection = Size = String.Empty;
      Criteria = new Criteria();
    }

    [JsonProperty("tile")]
    public int Tile { get; set; }

    [JsonProperty("size")]
    public string Size { get; set; }
    
    [JsonProperty("tag")]
    public string Tag
    {
      get
      {
        var tag = String.Format(
          "car.{0}.{1}{2}{3};prod={4};subprod={5};type={6};style={7};mak={8};mod={9};yr={10};fuel={11};tile={12};sz={13};",
          FirstLevel,
          Device,
          SecondLevel,
          ThirdLevel,
          Section,
          SubSection,
          Criteria.Category,
          Criteria.Style,
          Criteria.Make,
          Criteria.Model,
          Criteria.Year,
          Criteria.Fuel,
          Tile,
          Size);

        if (Tile == 11) //jag=ad1 mod for jumpstart mobile ad position - top
        {
          tag = String.Format(
          "car.{0}.{1}{2}{3};prod={4};subprod={5};type={6};style={7};mak={8};mod={9};yr={10};fuel={11};tile={12};jag=ad1;sz={13};",
          FirstLevel,
          Device,
          SecondLevel,
          ThirdLevel,
          Section,
          SubSection,
          Criteria.Category,
          Criteria.Style,
          Criteria.Make,
          Criteria.Model,
          Criteria.Year,
          Criteria.Fuel,
          Tile,
          Size);
        }
        else if (Tile == 12) //jag=ad2 mod for jumpstart mobile ad position - bottom
        {
          tag = String.Format(
          "car.{0}.{1}{2}{3};prod={4};subprod={5};type={6};style={7};mak={8};mod={9};yr={10};fuel={11};tile={12};jag=ad2;sz={13};",
          FirstLevel,
          Device,
          SecondLevel,
          ThirdLevel,
          Section,
          SubSection,
          Criteria.Category,
          Criteria.Style,
          Criteria.Make,
          Criteria.Model,
          Criteria.Year,
          Criteria.Fuel,
          Tile,
          Size);
        }
        
        return AdServerPrefix + tag;
      }
    }

    [JsonProperty("firstlevel")]
    public string FirstLevel { get; set; }

    [JsonProperty("device")]
    public string Device { get; set; }

    [JsonProperty("secondlevel")]
    public string SecondLevel { get; set; }
    
    [JsonProperty("thirdlevel")]
    public string ThirdLevel { get; set; }

    [JsonProperty("section")]
    public string Section { get; set; }

    [JsonProperty("subsection")]
    public string SubSection { get; set; }

    [JsonProperty("criteria")]
    public Criteria Criteria { get; set; }    

  }
}