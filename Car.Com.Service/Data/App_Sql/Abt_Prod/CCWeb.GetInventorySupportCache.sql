
/**!
 *	Proc Name:		  GetInventorySupportCache
 *	Lives In:		    Abt_Prod (REPL)
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	05/05/2015
 **/
 
USE Abt_Prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetInventorySupportCache' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventorySupportCache as select GetDate();')
GO


alter proc CCWeb.GetInventorySupportCache
 as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  /**
   * Return the Options recordset
   *
   * First Recordset Contract:
   *    { MatchValue, Description }
   *
   **/
  select 
    FeatureId as [MatchValue],  
    FeatureName as [Description]
  from UCI_Feature
  where ActiveFlag = 1
  


  /**
   * Return the FuelTypes recordset
   *
   * Second Recordset Contract:
   *    { MatchValue, Description }
   *
   **/
  select
    Fuel_Type_ID_int as [MatchValue],
    Description_Vch as [Description]
  from Fuel_Type
  where Active_bol = 1
  


  /**
   * Return the Dealer AutoCheck recordset
   *
   * Third Recordset Contract:
   *    { DealerId, AutoCheckId }
   *
   **/
  select
    DealerId, 
    AutoCheckId
  from Supp_AutoCheck


end 
GO


/*
  use abt_prod
  GO
  Drop procedure CCWeb.GetInventorySupportCache
  
  Exec CCWeb.GetInventorySupportCache

*/