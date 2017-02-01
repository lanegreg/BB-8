
/**!
 *	Proc Name:		  GetInventoryItemDetails
 *	Lives In:		    Abt_Prod
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	03/25/2015
 **/
 
USE abt_prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetInventoryItemDetails' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetInventoryItemDetails as select GetDate();')
GO


alter proc CCWeb.GetInventoryItemDetails (
  @InventoryId int
) as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted




  /**
   * Return the DealerCoverage recordset
   *
   * First Recordset Contract:
   *    { Id, InteriorColor, NumOfDoors, VehicleDetails, SellerNotes, DealerMessage }
   *
   **/
  select
    inv.Used_Car_ID_int as Id,
    inv.Interior_Color1_vch as InteriorColor,
    inv.Num_Doors_ti as NumOfDoors,
    inv.Addl_Features_vch as VehicleDetails,
    inv.Comments_vch  as SellerNotes,
    IsNull(dlr.Custom_message_vch, '') as DealerMessage
  from Avail_Used_Car as inv
  inner join Dealer as dlr 
    on dlr.Dealer_ID_int = inv.Dealer_ID_int
  where Used_Car_ID_int = @InventoryId

  
end 
GO


/*
  USE abt_prod
  GO
  
  Exec CCWeb.GetInventoryItemDetails 113818700

  Drop procedure CCWeb.GetInventoryItemDetails

*/