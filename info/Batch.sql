-----------------------------------------------
--BATCH
----------------------------------------------
/*
Generate receipt
*/
select * from dbo.ArrivalInstructions where id in (101,18,27)
select top 10 * from wms.WarehouseReceiptOrder where ScheduledArrivalNumber in (101,18,27) order by CreateAt desc
select top 10 * from wms.WarehouseReceiptOrderLine where ArrivalNo in (101,18,27) order by CreateAt desc

--select * from Products where ProductCode in ('CLOTH-001','ProductCodeTest2','HOME-001')
--select * from Products where CompanyId in (1,2)


--delete wms.WarehouseReceiptOrder where ScheduledArrivalNumber in (101,18,27)
--delete wms.WarehouseReceiptOrderLine where ArrivalNo in (101,18,27)
--update ArrivalInstructions set ProductCode ='BOOK-001' where id=101


select * from wms.WarehouseTrans
/*
Generate shippment
*/
--2669
select Id, CompanyId, CallingApiDeliveryStatus,FdaRegistrationStatus,IsCourierAssigned,* 
from dbo.OrderDispatches
where IsDeleted = 0
	and (CompanyId in (1)  and id in (2669,2235))
	and CallingApiDeliveryStatus in (1,-1)
	and FdaRegistrationStatus in (0,3)
	and IsCourierAssigned =1

select OrderId,OrderStatus,SubscriptionStatus,HadCheckAttachItem,OnHoldStatus,StockUpStatus,AutoSplitDeliveryStatus,*
from dbo.Orders 
where IsDeleted =0
	and OrderId in ('6055763575072','6071978721568')
	and OrderStatus= '20'
	and SubscriptionStatus!=-1
	and HadCheckAttachItem !=-1
	and OnHoldStatus in (0,2)
	and StockUpStatus =1
	and AutoSplitDeliveryStatus in (1,2)

	--select top 2 * from Orders order by CreateAt asc

select * from dbo.OrderDispatchProducts  where DispatchHeaderId in (2669,2235)

--fake data
--update OrderDispatches set CompanyId = 1, DeliveryCompanyCode = 'JP-YP' where id in (2235,2669)
--update Orders set OrderId ='6055763575072',CompanyId =1,OrderStatus='20', SubscriptionStatus=0,HadCheckAttachItem =0,OnHoldStatus=0, StockUpStatus =1,AutoSplitDeliveryStatus=1 where id =1800
--update Orders set OrderId ='6071978721568',CompanyId =1,OrderStatus='20', SubscriptionStatus=0,HadCheckAttachItem =0,OnHoldStatus=0, StockUpStatus =1,AutoSplitDeliveryStatus=1 where id =1867
--update OrderDispatchProducts set CompanyId = 1 where DispatchHeaderId in (2669,2235)
--update OrderDispatchProducts set ProductCode ='CLOTH-001', ShippedQty=10 where DispatchHeaderId = 2669
--update OrderDispatchProducts set ProductCode ='CLOTH-001', ShippedQty=11 where id = 378
--update OrderDispatchProducts set ProductCode ='SPORT-001', ShippedQty=12 where id = 539

select top 10 * from wms.WarehouseShipments order by CreateAt desc
select top 10 * from wms.WarehouseShipmentLines order by CreateAt desc
select top 10 * from wms.WarehouseTrans order by CreateAt desc
--WHS000168

select * from wms.WarehousePickingList order by CreateAt desc
select * from wms.WarehousePickingLines order by CreateAt desc

--delete wms.WarehouseShipments where ShipmentNo in('WHS000165','WHS000166')
--delete wms.WarehouseShipmentLines where ShipmentNo in('WHS000165','WHS000166')
--delete wms.WarehouseTrans where TransNumber in('WHS000165','WHS000166')

--delete wms.WarehouseShipments where OrderDispatchId in (2091)
--delete wms.WarehouseShipmentLines where ShipmentNo in ('WHS000126')
--update dbo.Orders set OrderStatus =20,SubscriptionStatus=0,HadCheckAttachItem=0,OnHoldStatus=0,StockUpStatus=1,AutoSplitDeliveryStatus=1 where OrderId = '6071978721568'

--update wms.WarehouseShipments set Status =1 where ShipmentNo in ('WHS000167','WHS000168')
--update wms.WarehouseShipmentLines set Status =1 where ShipmentNo in ('WHS000167','WHS000168')

--update wms.WarehouseShipments set ShippingCarrierCode ='JP-YP' where ShipmentNo in ('WHS000167','WHS000168')
----------------------------------------------------------------------------------------------------