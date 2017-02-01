
/**!
 *	Proc Name:		  GetInventoryCacheFull
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
    and p.name = N'GetInventoryCacheFull' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventoryCacheFull as select GetDate();')
GO


alter proc CCWeb.GetInventoryCacheFull (
  @LoadAmount int = null
) as
begin

  if(@LoadAmount is null)
    set @LoadAmount = 1000 -- Limit to 1 thousand recs, if not provided
    
  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  /**
   * Return the PageTemplateMeta recordset
   *
   * First Recordset Contract:
   *    { UrlPathPattern, Metadata }
   *
   **/
  select top(@LoadAmount) 
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
    cc.ImageUrls
  from dbo.Avail_Used_Car_Cache as cc
  
end 
GO


/*
USE Abt_Prod
GO

  Drop procedure CCWeb.GetInventoryCacheFull
  
  Exec CCWeb.GetInventoryCacheFull

*/