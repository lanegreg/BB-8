using Newtonsoft.Json;

namespace Car.Com.Domain.Models.Dealer
{
  public class TexasAdContent : ITexasAdContent
  {
    [JsonProperty("DealerId")]
    public int DealerId { get; set; }  

    [JsonProperty("ImageURL")]
    public string ImageUrl { get; set; }

    [JsonProperty("SupplierAdDesc")]
    public string SupplierAdDescription { get; set; }

    [JsonProperty("SupplierAdTypeId")]
    public int SupplierAdTypeId { get; set; }

    [JsonProperty("ImageWidth")]
    public int ImageWidth { get; set; }

    [JsonProperty("ImageHeight")]
    public int ImageHeight { get; set; }

    [JsonProperty("FrameWidth")]
    public int FrameWidth { get; set; }

    [JsonProperty("FrameHeight")]
    public int FrameHeight { get; set; }

    public Dto.Image Image
    {
      get
      {
        return new Dto.Image
        {
          Url = ImageUrl,
          Width = ImageWidth,
          Height = ImageHeight
        };
      }
    }

    public Dto.Frame Frame
    {
      get
      {
        return new Dto.Frame
        {
          Width = FrameWidth,
          Height = FrameHeight
        };
      }
    }


    public static class Dto
    {
      public class Image
      {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
      }

      public class Frame
      {
        public int Width { get; set; }
        public int Height { get; set; }
      }
    }
  }
}
