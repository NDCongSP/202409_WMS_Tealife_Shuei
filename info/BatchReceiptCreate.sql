/****** Script for SelectTopNRows command from SSMS  ******/

select * from dbo.ArrivalInstructions where ScheduledArrivalNumber = '5'

SELECT *
  FROM [FBT_DEV2].[wms].[WarehouseReceiptOrder]
  where ScheduledArrivalNumber = '5'

  SELECT *
  FROM [FBT_DEV2].[wms].[WarehouseReceiptOrderLine]
  where ArrivalNo ='5'

  --update wms.WarehouseReceiptOrder set ScheduledArrivalNumber =null where ScheduledArrivalNumber ='5'
  --update wms.WarehouseReceiptOrderLine set ArrivalNo =null where ArrivalNo ='5'