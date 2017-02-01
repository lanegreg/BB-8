
/**!
 *	Proc Name:		  GetCityListByZipcode
 *	Lives In:		    Abt_Prod
 *  Schema Name:    CCWeb
 *
 *	Author:			    Bill Ray
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	01.27/2015
 **/
 
USE abt_prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetCityListByZipcode' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetCityListByZipcode as select GetDate();')
GO


alter proc CCWeb.GetCityListByZipcode (
  @Zipcode varchar(5)
) as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted

SELECT  c.Description_vch  AS City,  
        s.State_Abbrev_ch  AS StateAbbreviation
FROM	dbo.Area a   
JOIN	dbo.City c        
ON		c.City_ID_int = a.City_ID_int  
JOIN	dbo.State s        
ON		s.State_ID_int   = a.State_ID_int
WHERE	a.Postal_Code_ch = @Zipcode    
AND		a.active_bol = 1  
AND		c.active_bol = 1  
AND		s.active_bol = 1  

  
end 
GO


/*
  USE abt_prod
  GO
  Drop procedure abt_prod.CCWeb.GetCityListByZipcode
  
  Exec abt_prod.CCWeb.GetCityListByZipcode 92627

*/