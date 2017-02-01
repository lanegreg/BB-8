
/**!
 *	Proc Name:		  GetDealerCoverage
 *	Lives In:		    Abt_Prod
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	12/08/2014
 **/
 
USE abt_prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetDealerCoverage' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetDealerCoverage as select GetDate();')
GO


alter proc CCWeb.GetDealerCoverage (
  @Zipcode varchar(5)
) as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  declare @Latitude float,
          @Longitude float

  select
    @Latitude = Centroid_Lat_r,
		@Longitude = Centroid_Lon_r
  from dbo.Postal_Location   
  where Postal_Code_vch = @Zipcode 



  /**
   * Return the DealerCoverage recordset
   *
   * First Recordset Contract:
   *    { DealerId, Distance }
   *
   **/
  select distinct
		DealerId,
		Distance
  from (
	  select
      DealerId,
			DealerMiles,
			cast(dbo.UDF_GetTravelDistance(@Latitude, @Longitude, CentroidLat, CentroidLon) as int) as [Distance]
		from dbo.Avail_Used_Car_Dealer_Cache
  ) as subquery
  where Distance < DealerMiles
  order by Distance

  
end 
GO


/*
  USE abt_prod
  GO
  Drop procedure abt_prod.CCWeb.GetDealerCoverage
  
  Exec abt_prod.CCWeb.GetDealerCoverage 92627

*/