
namespace Car.Com.Domain.Models.Lead
{
  public interface IDealer
  {
    int Id { get; }
    string Name { get; }
    string Address { get; }
    string City { get; }
    string State { get; }
    string Zip { get; }
    string Message { get; }
    string Phone { get; }
    string LogoUrl { get; }
    string LogoWidth { get; }
    string LogoHeight { get; }
    int ConfirmationNum { get; }
  }
}