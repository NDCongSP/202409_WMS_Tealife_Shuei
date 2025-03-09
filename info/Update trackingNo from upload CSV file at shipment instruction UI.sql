select OrderId --お客様側管理番号 → Customer Management Number
	,TrackingNo --お問い合わせ番号 → Inquiry Number
	,LabelFilePath,LabelFileExtension, * 
from dbo.OrderDispatches
where TrackingNo ='000005'

select InternalRemarks, *
from dbo.Orders
where OrderId ='5996835143968'
order by CreateAt asc

--update dbo.Orders set InternalRemarks='Cautions Cautions Cautions Cautions ' where id =1800

select *
from wms.WarehouseShipments
where ShipmentNo ='WHS000031'

select trackingno,labelfilepath, * from dbo.orderdispatches where DeliveryId='D20000002195' and IsMarketUpdated = 0
select * from wms.WarehouseShipments where ShipmentNo in ('WHS000046') and ShippingCarrierCode='JP-YP' and Status=5

--update wms.WarehouseShipments set TrackingNo = null where ShipmentNo='WHS000046'
--update dbo.OrderDispatches set TrackingNo = null  where id= 2063


update dbo.OrderDispatches.TrackingNo = CSV.TrackignNo WHERE OrderDispatches.DeliveryId = CSV.DeliveryId and  [IsMarketUpdated] = '0'

declare @__customerManagementNumbers_0 nvarchar(max) = '["D20000002195","5460372455470"]'
SELECT [o].[Id], [o].[CallingApiDeliveryStatus], [o].[ChannelCode], [o].[CompanyId], [o].[CreateAt], [o].[CreateOperatorId], [o].[CutoffDate], [o].[CutoffId], [o].[DataKey], [o].[DeliveryCompanyCode], [o].[DeliveryId], [o].[DispatchStatus], [o].[FdaRegistrationStatus], [o].[InvoiceFileExtension], [o].[InvoiceFilePath], [o].[IsCourierAssigned], [o].[IsCutOff], [o].[IsDeleted], [o].[IsMarketShipped], [o].[IsMarketUpdated], [o].[LabelFileExtension], [o].[LabelFilePath], [o].[MarketDeliveryNo], [o].[OrderDispatchStatus], [o].[OrderId], [o].[ReferenceId], [o].[ShipmentDate], [o].[StockUpStatus], [o].[TrackingNo], [o].[UpdateAt], [o].[UpdateOperatorId]
FROM [OrderDispatches] AS [o]
WHERE [o].[DeliveryId] IN (
    SELECT [c].[value]
    FROM OPENJSON(@__customerManagementNumbers_0) WITH ([value] nvarchar(max) '$') AS [c]
)