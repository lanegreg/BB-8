using System.Collections.Generic;
using Car.Com.Domain.Models.Translators;

namespace Car.Com.Domain.Services
{
  public interface IUriTokenTranslators
  {
    IEnumerable<IMakeTranslator> GetAllMakeTranslators();
    IEnumerable<IModelTranslator> GetAllModelTranslators();
    IEnumerable<ICategoryTranslator> GetAllCategoryTranslators();
    IMakeTranslator GetMakeTranslatorBySeoName(string seoName);
    IModelTranslator GetModelTranslatorBySeoName(string seoName);
    ICategoryTranslator GetCategoryTranslatorBySeoName(string seoName);
  }
}
