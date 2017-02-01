
/**!
 *	Proc Name:		  GetInventoryCache
 *	Lives In:		    CacheDB (REPL)
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	04/28/2015
 **/
 
USE CacheDB
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetInventoryCache' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventoryCache as select GetDate();')
GO


alter proc CCWeb.GetInventoryCache(
  @LoadAmount int = 100
) as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  /**
   * Return the Dealers recordset
   *
   * First Recordset Contract:
   *    { Id, ProgramId, Name, Phone, City, State, RevenueScore, IsPremiumPlacement }
   *
   **/
  select 
    DealerId as [Id],
    ProgramId as [ProgramId],
    DealerName as [Name],
    City as [City],
    [State] as [State],
    IsNull(PhoneNumber,'') as [Phone],
    RevenueScore as [RevenueScore],
    PremiumPlacement as [IsPremiumPlacement]
  from DealerCache
  order by DealerId
  
  
  
  
  /**
   * Return the CarsForSale recordset
   *
   * Second Recordset Contract:
   *    { Id, ProgramId, DealerId, Vin, Year, MakeId, Make, ModelId, Model, Trim, DriveType, CategoryId, 
   *      FuelTypeId, Mileage, AskingPrice, ExteriorColor, Cylinders, TransmissionType, CityMpg, HighwayMpg, 
   *      OptionBits, PipeSeparatedImageUrls, IsNewStatus, TrimId, DisplayName, SquishVin }
   *
   **/
  select top (@LoadAmount)
    UsedCarId as [Id],
    ProgramId,
    DealerId,
    Vin,
    [Year],
    MakeId,
    Make,
    ModelId,
    Model,
    Series as [Trim],
    DriveId as [DriveType],
    VehicleClassId as [CategoryId],
    FuelTypeId,
    Mileage,
    AskingPrice,
    ExteriorColor,
    Cylinders,
    TransmissionType,
    IsNull(cast(CityMpg as varchar), 'N/A') as [CityMpg],
    IsNull(cast(HwyMpg as varchar), 'N/A') as [HighwayMpg],
    FeatureBits as [OptionBits],
    IsNull(ImageUrls, '') as [PipeSeparatedImageUrls],
    IsNew as [IsNewStatus],
    TrimId,
    DisplayName = FullDisplayName,
    SquishVin
  from Inventory
  order by DealerId
  


end 
GO


/*
  use CacheDB
  GO
  Drop procedure CCWeb.GetInventoryCache
  
  Exec CCWeb.GetInventoryCache 10000

*/