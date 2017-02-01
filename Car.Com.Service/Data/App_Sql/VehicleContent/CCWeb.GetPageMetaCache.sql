
/**!
 *	Proc Name:		  GetPageMetaCache
 *	Lives In:		    VehicleContent
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	01/05/2015
 **/
 
USE VehicleContent
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetPageMetaCache' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetPageMetaCache as select GetDate();')
GO


alter proc CCWeb.GetPageMetaCache
 as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  /**
   * Return the PageTemplateMeta recordset
   *
   * First Recordset Contract:
   *    { UrlPathPattern, Metadata }
   *
   **/
	select 
    UrlPathPattern, 
    Metadata
  from CCWeb.PageTemplateMeta
  
end 
GO


/*
USE VehicleContent
GO

  Drop procedure CCWeb.GetPageMetaCache
  
  Exec CCWeb.GetPageMetaCache

*/