using System;
using Car.Com.Domain.Models.SiteMeta;
using System.Collections.Generic;

namespace Car.Com.Models.Research
{
  public class TrimIncentivesViewModel : ViewModelBase
  {
    public TrimIncentivesViewModel(string assetsPrefix)
      : base(assetsPrefix)
    {
    }

    public TrimIncentivesViewModel(string assetsPrefix, IMetadata metadata)
      : base(assetsPrefix, metadata)
    {
    }

    public string TrimSectionSeoName { get; set; }
    public int Year { get; set; }
    public Dto.Make Make { get; set; }
    public Dto.SuperModel SuperModel { get; set; }
    public Dto.Trim Trim { get; set; }
    public IEnumerable<Dto.Incentives> Public { get; set; }
    public IEnumerable<Dto.Incentives> Retiree { get; set; }
    public IEnumerable<Dto.Incentives> Military { get; set; }
    public IEnumerable<Dto.Incentives> College { get; set; }

    public static class Dto
    {
      public class Make
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class SuperModel
      {
        public string Name { get; set; }
        public string SeoName { get; set; }
      }

      public class Trim
      {
        public int TrimId { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string FullDisplayName { get; set; }
        public string SeoName { get; set; }
        public string SuperTrim { get; set; }
        public string Msrp { get; set; }
      }

      public class Incentives
      {
        public string Expires { get; set; }
        public string GroupDesc { get; set; }
        public string CatDesc { get; set; }
        public string MasterDesc { get; set; }
        public decimal Amount { get; set; }
        public double APR_24 { get; set; }
        public double APR_36 { get; set; }
        public double APR_48 { get; set; }
        public double APR_60 { get; set; }
        public double APR_72 { get; set; }
        public double APR_84 { get; set; }
      }

    }
  }
}
