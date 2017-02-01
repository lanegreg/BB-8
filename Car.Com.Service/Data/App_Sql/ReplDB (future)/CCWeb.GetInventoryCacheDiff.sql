
/**!
 *	Proc Name:		  GetInventoryCacheDiff
 *	Lives In:		    Abt_Prod (REPL)
 *  Schema Name:    CCWeb
 *
 *	Author:			    Doug Ellner | Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	01/09/2015
 **/
 
USE Abt_Prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetInventoryCacheDiff' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventoryCacheDiff as select GetDate();')
GO


alter proc CCWeb.GetInventoryCacheDiff (
  @Version bigint
) as
begin

  select 
    cc.UsedCarId, 
    cc.ProgramId, 
    cc.DealerId, 
    cc.VIN, 
    cc.[Year], 
    cc.MakeId, 
    cc.ModelId, 
    cc.SeriesId, 
    cc.Make, 
    cc.Model, 
	  cc.Series, 
    cc.DriveId, 
    cc.VehicleClassId, 
    cc.FuelTypeId, 
    cc.Mileage, 
    cc.AskingPrice, 
    cc.ExteriorColor, 
    cc.Cylinders, 
	  cc.TransmissionType, 
    cc.CityMPG, 
    cc.HwyMPG, 
    cc.FeatureBits, 
    cc.ImageUrls,
    SYS_CHANGE_VERSION, 
    SYS_CHANGE_OPERATION
  from dbo.Avail_Used_Car_Cache as cc
  right join CHANGETABLE (CHANGES Avail_Used_Car_Cache, @Version) as ct 
    on ct.UsedCarId = cc.UsedCarId;
  
end 
GO


/*
USE Abt_Prod
GO

  Drop procedure CCWeb.GetInventoryCacheDiff
  
  Exec CCWeb.GetInventoryCacheDiff @Version = 4

*/