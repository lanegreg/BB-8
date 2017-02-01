using Car.Com.Domain.Models.Evaluation;
using System.Collections.Generic;

namespace Car.Com.Domain.Services
{
  public interface IEvaluationService
  {
    IEnumerable<EvaluationType> EvaluationTypes { get; }

    IEnumerable<Year> GetYears();
    IEnumerable<Make> GetMakesByYear(int year);
    IEnumerable<Trim> GetTrimsByYearByMakeByEvaluationType(int year, string makeKey, string evalTypeKey);
    IEnumerable<Feature> GetFeaturesByTrimByFeatureType(string trimKey, FeatureType featureType);
    string GetVehicleValue(string trimKey, string valueType, string conditionType, string mileage, string zipCode, string equipments);
    Evaluation GetEvaluation(Criteria criteria);
  }
}
