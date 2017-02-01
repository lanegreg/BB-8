using Car.Com.Domain.Models.VehicleSpec;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Com.Domain.Services
{
  public interface IVehicleSpecService
  {
    Task<IEnumerable<IMake>> GetAllActiveMakesAsync();
    IEnumerable<IMake> GetAllActiveMakes();
    Task<IEnumerable<IMake>> GetAllMakesAsync();
    IEnumerable<IMake> GetAllMakes();

    Task<IEnumerable<IModel>> GetAllModelsAsync();
    IEnumerable<IModel> GetAllModels();

    Task<IEnumerable<Year>> GetAllYearsAsync();
    IEnumerable<Year> GetAllYears();
    
    Task<IEnumerable<Year>> GetAllResearchYearsAsync();
    IEnumerable<Year> GetAllResearchYears();

    Task<IEnumerable<ICategory>> GetAllCategoriesAsync();
    IEnumerable<ICategory> GetAllCategories();

    Task<IEnumerable<ICategory>> GetAllAbtCategoriesAsync();
    IEnumerable<ICategory> GetAllAbtCategories();
    
    Task<IEnumerable<ISuperModel>> GetAllNewSuperModelsAsync();
    IEnumerable<ISuperModel> GetAllNewSuperModels();

    Task<IEnumerable<ISuperModel>> GetAllUsedSuperModelsAsync();
    IEnumerable<ISuperModel> GetAllUsedSuperModels();
    
    Task<IEnumerable<ISuperModel>> GetNewSuperModelsByMakeAsync(string modelSeoName);
    IEnumerable<ISuperModel> GetNewSuperModelsByMake(string modelSeoName);
    
    Task<IEnumerable<IUniqueTrim>> GetAllUniqueTrimsAsync();
    IEnumerable<IUniqueTrim> GetAllUniqueTrims();

    Task<IEnumerable<IModel>> GetModelsByMakeAsync(string makeSeoName);
    IEnumerable<IModel> GetModelsByMake(string makeSeoName);

    Task<IEnumerable<ITrim>> GetTrimsByCategoryAsync(string categorySeoName);
    IEnumerable<ITrim> GetTrimsByCategory(string categorySeoName);

    Task<IEnumerable<IVehicleAttribute>> GetAllVehicleAttributeNamesAsync();
    IEnumerable<IVehicleAttribute> GetAllVehicleAttributeNames();

    Task<IEnumerable<ITrim>> GetTrimsByVehicleAttributeNameAsync(string vehicleAttributeSeoName);
    IEnumerable<ITrim> GetTrimsByVehicleAttributeName(string vehicleAttributeSeoName);
    
    Task<IEnumerable<ITrim>> GetTrimsByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName);
    IEnumerable<ITrim> GetTrimsByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName);
    
    Task<IEnumerable<ITrim>> GetNewTrimsByMakeBySuperModelAsync(string makeSeoName, string superModelSeoName);
    IEnumerable<ITrim> GetNewTrimsByMakeBySuperModel(string makeSeoName, string superModelSeoName);

    Task<IEnumerable<ITrim>> GetTrimsByMakeBySuperModelByYearAsync(string makeSeoName, string superModelSeoName, int yearNumber);
    IEnumerable<ITrim> GetTrimsByMakeBySuperModelByYear(string makeSeoName, string superModelSeoName, int yearNumber);

    Task<IEnumerable<ITrim>> GetSimilarTrimsByTrimIdAsync(int trimId);
    IEnumerable<ITrim> GetSimilarTrimsByTrimId(int trimId);

    Task<IEnumerable<ITrim>> GetSimilarTrimsByPriceAsync(int price);
    IEnumerable<ITrim> GetSimilarTrimsByPrice(int price);

    Task<ITrim> GetAbtTrimsByAcodeAsync(string acode);
    ITrim GetAbtTrimsByAcode(string acode);

    Task<ITrim> GetTrimOverviewByMakeBySuperModelByYearByTrimAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);
    ITrim GetTrimOverviewByMakeBySuperModelByYearByTrim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);

    Task<ITrim> GetTrimSpecsByMakeBySuperModelByYearByTrimAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);
    ITrim GetTrimSpecsByMakeBySuperModelByYearByTrim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);

    Task<ITrim> GetTrimSafetyByMakeBySuperModelByYearByTrimAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);
    ITrim GetTrimSafetyByMakeBySuperModelByYearByTrim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);

    Task<ITrim> GetTrimColorByMakeBySuperModelByYearByTrimAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);
    ITrim GetTrimColorByMakeBySuperModelByYearByTrim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);

    Task<ITrim> GetTrimIncentivesByMakeBySuperModelByYearByTrimByZipAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName, string zip);
    ITrim GetTrimIncentivesByMakeBySuperModelByYearByTrimByZip(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName, string zip);

    Task<IEnumerable<TrimIncentive>> GetTrimIncentivesByTrimIdByZipAsync(int trimId, string zip);
    IEnumerable<TrimIncentive> GetTrimIncentivesByTrimIdByZip(int trimId, string zip);

    Task<IEnumerable<Trim>> GetCompareCarsByTrimIdListAsync(string trimIdList);
    IEnumerable<Trim> GetCompareCarsByTrimIdList(string trimIdList);

    Task<IEnumerable<ISpecification>> GetTrimSpecOptionsByMakeBySuperModelByYearByTrimAsync(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);
    IEnumerable<ISpecification> GetTrimSpecOptionsByMakeBySuperModelByYearByTrim(string makeSeoName, string superModelSeoName, int yearNumber, string trimSeoName);

    Task<IEnumerable<ICategoryFilterGroup>> GetCategoryFilterGroupDataByCategoryAsync(string categorySeoName);
    IEnumerable<ICategoryFilterGroup> GetCategoryFilterGroupDataByCategory(string categorySeoName);
    
    Task<IEnumerable<ICategory>> GetCategoriesByVehicleAttributeSeoNameAsync(string vehicleAttributeSeoName);
    IEnumerable<ICategory> GetCategoriesByVehicleAttributeSeoName(string vehicleAttributeSeoName);

    Task<IEnumerable<ICategoryFilterGroup>> GetCustomFilterGroupDataByVehicleAttributeNameAsync(string vehicleAttributeSeoName);
    IEnumerable<ICategoryFilterGroup> GetCustomFilterGroupDataByVehicleAttributeName(string vehicleAttributeSeoName);

    Task<IEnumerable<ICategoryFilterGroup>> GetCustomFilterGroupDataByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName);
    IEnumerable<ICategoryFilterGroup> GetCustomFilterGroupDataByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName);

    Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCustomTrimFilterAttributesByVehicleAttributeNameAsync(string vehicleAttributeSeoName);
    IEnumerable<ICategoryTrimFilterAttribute> GetCustomTrimFilterAttributesByVehicleAttributeName(string vehicleAttributeSeoName);

    Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeNameAsync(string categorySeoName, string vehicleAttributeSeoName);
    IEnumerable<ICategoryTrimFilterAttribute> GetCustomTrimFilterAttributesByCategoryAndVehicleAttributeName(string categorySeoName, string vehicleAttributeSeoName);
    
    Task<IEnumerable<ICategoryTrimFilterAttribute>> GetCategoryTrimFilterAttributesByCategoryAsync(string categorySeoName);
    IEnumerable<ICategoryTrimFilterAttribute> GetCategoryTrimFilterAttributesByCategory(string categorySeoName);

    Task<IEnumerable<IVehicleAttribute>> GetVehicleAttributesForAltFuelTrimsAsync();
    IEnumerable<IVehicleAttribute> GetVehicleAttributesForAltFuelTrims();
    
    Task<IEnumerable<ISuperModelFilterGroup>> GetSuperModelFilterGroupDataByMakeSuperModelAsync(string makeSeoName, string superModelSeoName);
    IEnumerable<ISuperModelFilterGroup> GetSuperModelFilterGroupDataByMakeSuperModel(string makeSeoName, string superModelSeoName);

    Task<IEnumerable<ISuperModelTrimFilterAttribute>> GetSuperModelTrimFilterAttributesByMakeSuperModelAsync(string makeSeoName, string superModelSeoName);
    IEnumerable<ISuperModelTrimFilterAttribute> GetSuperModelTrimFilterAttributesByMakeSuperModel(string makeSeoName, string superModelSeoName);

    Task<IEnumerable<ISuperModelFilterGroup>> GetSuperModelFilterGroupDataByMakeSuperModelByYearAsync(string makeSeoName, string superModelSeoName, int yearNumber);
    IEnumerable<ISuperModelFilterGroup> GetSuperModelFilterGroupDataByMakeSuperModelByYear(string makeSeoName, string superModelSeoName, int yearNumber);

    Task<IEnumerable<ISuperModelTrimFilterAttribute>> GetSuperModelTrimFilterAttributesByMakeSuperModelByYearAsync(string makeSeoName, string superModelSeoName, int yearNumber);
    IEnumerable<ISuperModelTrimFilterAttribute> GetSuperModelTrimFilterAttributesByMakeSuperModelByYear(string makeSeoName, string superModelSeoName, int yearNumber);
  }
}