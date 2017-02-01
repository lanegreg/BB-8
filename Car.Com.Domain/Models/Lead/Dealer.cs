
namespace Car.Com.Domain.Models.Lead
{
  public class Dealer : IDealer
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Message { get; set; }
    public string Phone { get; set; }
    public string LogoUrl { get; set; }
    public string LogoWidth { get; set; }
    public string LogoHeight { get; set; }
    public int ConfirmationNum { get; set; }
  }
}
