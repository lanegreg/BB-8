using Newtonsoft.Json;

namespace Car.Com.Domain.Models.Dealer
{
  public class DisplayContent : IDisplayContent
  {
    [JsonProperty("DealerId")]
    public int DealerId { get; set; }

    [JsonProperty("CustomMessage")]
    public string DealerMessage { get; set; }

    [JsonProperty("ThankYouMessage")]
    public string ThankYouMessage { get; set; }

    [JsonProperty("LogoImage")]
    public string LogoImage { get; set; }

    [JsonProperty("LogoHeight")]
    public int LogoHeight { get; set; }

    [JsonProperty("LogoWidth")]
    public int LogoWidth { get; set; }

    public Dto.Logo Logo
    {
      get
      {
        return new Dto.Logo
        {
          Url = LogoImage,
          Width = LogoWidth,
          Height = LogoHeight
        };
      }
    }


    public static class Dto
    {
      public class Logo
      {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
      }
    }
  }
}
