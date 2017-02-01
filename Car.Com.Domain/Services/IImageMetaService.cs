using Car.Com.Domain.Models.Image;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface IImageMetaService
  {
    Task<IEnumerable<IImageMeta>> GetImagesByTrimIdsAsync(List<int> trimIds);
    IEnumerable<IImageMeta> GetImagesByTrimIds(List<int> trimIds);
    
    Task<IEnumerable<IImageMeta>> GetImagesByTrimIdAsync(int trimId);
    IEnumerable<IImageMeta> GetImagesByTrimId(int trimId);

    Task<IEnumerable<IImageMeta>> GetImagesByMakeByModelByYearAsync(string makeSeoName, string modelSeoName, int yearNumber);
    IEnumerable<IImageMeta> GetImagesByMakeByModelByYear(string makeSeoName, string modelSeoName, int yearNumber);
  }
}
