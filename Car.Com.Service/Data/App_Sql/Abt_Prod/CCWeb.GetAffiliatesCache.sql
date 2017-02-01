
/**!
 *	Proc Name:		  GetAffiliatesCache
 *	Lives In:		    Abt_Prod
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	12/01/2014
 **/
 
USE abt_prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetAffiliatesCache' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetAffiliatesCache as select GetDate();')
GO


alter proc CCWeb.GetAffiliatesCache
 as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted


  /**
   * Return the Affiliates recordset
   *
   * First Recordset Contract:
   *    { Id, Name, GroupName }
   *
   **/
  select
    src.PR_Source_ID_int as [Id],
    src.Description_vch as [Name],
    grp.Description_vch as [GroupName]
  from abt_prod..PR_Source as src
  inner join abt_prod..PR_Source_Group as grp
    on grp.PR_Source_Group_id_int = src.PR_Source_Group_id_int
      and grp.Active_bol = 1
  order by src.PR_Source_ID_int
  
end 
GO


/*

  Drop procedure abt_prod.CCWeb.GetAffiliatesCache
  
  Exec abt_prod.CCWeb.GetAffiliatesCache

*/