
/**!
 *	Proc Name:		  GetInventoryCache
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
    and p.name = N'GetInventoryCache' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventoryCache as select GetDate();')
GO


alter proc CCWeb.GetInventoryCache (
  @Version bigint = null OUTPUT,
	@Server varchar(50) = null OUTPUT,
  @Top int = null
) as
begin

  SET NOCOUNT ON

  declare @CurrentVersion bigint, 
          @CurrentServer varchar(50)

  set @CurrentVersion = CHANGE_TRACKING_CURRENT_VERSION()
  set @CurrentServer = @@SERVERNAME

  if @CurrentServer like '%DEV%' and @Version is NOT null and @Version > @CurrentVersion
    begin
	        RAISERROR ('The @Version parameter is greater than the current change tracking version.', 16, 1);
    end

  SET NOCOUNT OFF 

  if(@Top is null)
    set @Top = 10000 -- Limit to 10 thousand recs



  /**!
   * If @Version OR @Server are NULL then we are performing an initial load.
   *
   * If @Server is not the same as the server running this procedure then we must assume that we have a 
   * failover event.  Since the change tracking current version may be different on each scale out
   * database server we must perform a full refresh.
   *
   **/

  if @Version is null or @Server is null or @Server != @CurrentServer -- Full refresh
    begin
	    exec CCWeb.GetInventoryCacheFull 
	    set @Server = @CurrentServer 
    end
  else
    begin
	    exec CCWeb.GetInventoryCacheDiff @Version
    end

  set @Version = @CurrentVersion
  
end 
GO


/*
USE Abt_Prod
GO

  Drop procedure CCWeb.GetInventoryCache
  
  Exec CCWeb.GetInventoryCache @Version = 4, @Server = null

*/