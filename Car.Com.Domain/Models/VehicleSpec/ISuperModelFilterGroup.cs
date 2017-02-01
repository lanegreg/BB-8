﻿using System;
using System.Collections.Generic;

namespace Car.Com.Domain.Models.VehicleSpec
{
  public interface ISuperModelFilterGroup
  {
    string FilterGroupName { get; }
    string SvgId { get; }
    string DisplayName { get; }
    string Code { get; }
  }
}
