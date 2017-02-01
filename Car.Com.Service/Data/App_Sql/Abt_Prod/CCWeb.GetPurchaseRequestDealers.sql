
/**!
 *	Proc Name:		  GetPurchaseRequestDealers
 *	Lives In:		    Abt_Prod
 *  Schema Name:    CCWeb
 *
 *	Author:			    Bill Ray
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	02/09/2015
 **/
 
USE abt_prod
GO


if NOT exists (
  select * 
  from sys.procedures as p 
  left join sys.schemas as s on p.schema_id = s.schema_id 
  where s.Name = 'CCWeb'
    and p.name = N'GetPurchaseRequestDealers' 
    and p.type in (N'P', N'PC')
)
exec('create proc CCWeb.GetPurchaseRequestDealers as select GetDate();')
GO


alter proc CCWeb.GetPurchaseRequestDealers(
  @prnumber int
) as 
begin

  -- Dirty reads are OK
  set transaction isolation level read uncommitted

		select
			dlr.Dealer_Id_int as Id,
			dlr.DBA_vch as Name,
			dlr.Street_Address_1_vch as Address,
			dan.City_Description_vch as City,
			dan.State_Abbrev_ch as State,
			dan.Postal_Code_ch as Zip,
			dlr.Thank_You_Message_vch as Message,
			IsNull(
				Case when req.Vehicle_Type_ch = 'N'
					then (
						select top 1 ct.Work_Phone_vch
						from ABT_Prod.dbo.Contact ct (NoLock)
						where ct.Domain_fk_int = prf.Supplier_Id_int
							and ct.Contact_Type_ch = 'DEAL'
							and ct.Title_Id_int	= 3
							and ct.Active_Acc_Flag_bol = 1
					)			
					else (
						select top 1 ct.Work_Phone_vch
						from ABT_Prod.dbo.Contact ct (NoLock)
						where ct.Domain_fk_int = prf.Supplier_Id_int
							and ct.Contact_Type_ch = 'DEAL'
							and ct.Title_Id_int = 34
							and ct.Active_Acc_Flag_bol = 1
					)		
				end,
				dlr.Voice_Phone_vch
			) as Phone,
			dlr.Logo_Image_vch as LogoUrl,
			dlr.Logo_Width_int as LogoWidth,
			dlr.Logo_Height_int as LogoHeight
		from ABT_Prod.dbo.Purchase_Request req (NoLock)
		join ABT_Prod.dbo.PR_Fulfillment prf (NoLock) on prf.PR_Number_Id_int = req.PR_Number_Id_int
			and req.PR_Number_Id_int = @prnumber
		left join ABT_Prod.dbo.Dealer dlr (NoLock) on dlr.Dealer_Id_int = prf.Supplier_Id_int
			and prf.Supplier_Type_ID_int = 1
		left join ABT_Prod.dbo.Area_Names dan (NoLock) on dan.Area_Id_int = dlr.Loc_Area_Id_int
		where dlr.Dealer_Id_int is NOT null

  
end 
GO


/*
  USE abt_prod
  GO
  Drop procedure abt_prod.CCWeb.GetPurchaseRequestDealers
  
  Exec abt_prod.CCWeb.GetPurchaseRequestDealers 92627

*/