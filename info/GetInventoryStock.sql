/****** Script for SelectTopNRows command from SSMS  ******/
--WHR000110
--delete wms.WarehouseTrans where TransNumber = 'WHR000109'

SELECT *
  FROM [FBT_DEV2].[wms].WarehouseTrans
  where ProductCode = 'PRO999'
	--and DatePhysical is not null
	--and Bin = 'B001'
	--and LotNo is null
  -- (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
  order by CreateAt desc

  exec sp_refreshview 'view_wmsGetInventoryInfoFlowBinLot' 
  exec sp_refreshview 'view_wmsGetInventoryInfo' 

--exec sp_getInventoryInfoFlowBinLot
select BaseCost,BaseCostOther, * from dbo.Products 
where ProductCode in ('SIDA001')
order by CreateAt desc

select * from wms.[Batches] where ProductCode='PRO999' and LotNo='LOT20241114'
select * from wms.WarehouseReceiptOrder where ReceiptNo in ('WHR000175') order by CreateAt desc
select * from wms.WarehouseReceiptOrderLine where ReceiptNo in ('WHR000175') order by CreateAt desc
select * from wms.WarehouseShipments where ShipmentNo='WHS000040'
select * from wms.WarehouseTrans where ProductCode in ('PRO999') order by CreateAt desc
--select  * from [FBT_DEV2].dbo.view_wmsGetInventoryInfoFlowBinLot where ProductCode= 'PRO999'
exec sp_getInventoryInfoFlowBinLot @productCode = 'PRO999', @companyId = 4
exec sp_getInventoryInfo

exec sp_productGetDataMaster

exec sp_packingGetShipmentByTrackingNo @trackingNo = '000005'
exec sp_packingGetShipmentByShipmentNo @shipmentNo = 'WHS000007'
exec sp_packingGetShipmentByShipmentNo @shipmentNo = 'WHS000031'

select trackingno,labelfilepath, * from dbo.orderdispatches where trackingNo in ('000005') or DeliveryId ='D20000002195'
select * from wms.WarehouseShipments where ShipmentNo in ('WHS000031','WHS000046')--in ('WHS000031','WHS000007','WHS000046','WHS000057')
select * from wms.WarehouseShipmentLines where ShipmentNo in ('WHS000031','WHS000046')

select * from wms.WarehousePickingStaging where ShipmentLineId = 'F5C7FF7A-340A-4FE5-9842-2D6756D2A96A'and ProductQRCode like 'AC001-000-100:JanCode:LOT1234::2025-12-10%'

----------------------------------------Receipt--------------------------------------------------------------
select top 10 * from wms.WarehouseReceiptOrder order by CreateAt desc
----------------------------------------End receipt---------------------------------------------------------

---------------------------------------get data master fo picking creating------------------------------------
select * from wms.WarehouseShipments where ShipmentNo in ('WHS000168','WHS000169','WHS000170','WHS000171','WHS000172') order by CreateAt desc
select * from wms.WarehouseShipmentLines where ShipmentNo in ('WHS000168','WHS000169','WHS000170','WHS000171','WHS000172') order by CreateAt desc
select * from wms.WarehousePickingList order by CreateAt desc
select * from wms.WarehousePickingLines order by CreateAt desc
select * from wms.NumberSequences
--select * from wms.WarehouseTrans order by CreateAt desc
exec sp_GetDataForCreatePicking @shipmentId='550DB25A-C222-4B05-97C6-4F76CC20C464,0DF29687-1D4F-4EC5-13CB-08DD5D84253F,A8EE991B-99AA-4E65-13CA-08DD5D84253F,1D2403BB-239F-416C-13C9-08DD5D84253F,1FE285D5-C459-428C-F6F5-08DD5DDCEF86', @companyId= 0


select * from wms.WarehouseShipments where PickingNo='WHPI000173'
select * from wms.WarehousePickingList where PickNo ='WHPI000173'
select * from wms.WarehousePickingLines  where PickNo ='WHPI000173'
select * from wms.WarehousePickingStaging

--update wms.WarehouseShipments set Status = 1 where ShipmentNo in ('WHS000168','WHS000169','WHS000170','WHS000171','WHS000172')
--update wms.WarehouseShipmentLines	set Status =1  where ShipmentNo in ('WHS000168','WHS000169','WHS000170','WHS000171','WHS000172')
--truncate table wms.warehousepickingList
--truncate table wms.warehousepickingLines
-------------------------------------------------------------------------------------------------------------------------------------------------

--delete wms.WarehouseShipmentLines  where ShipmentNo in ('WHS000151','WHS000152','WHS000153')

--update wms.WarehousePickingStaging set [Location]='ce3045c1-74f9-4e33-b358-5a183b3afd69',Bin='B001',LotNo='LOT1234',UnitId=1,ShipmentLineId='F5C7FF7A-340A-4FE5-9842-2D6756D2A96A',ProductQRCode='AC001-000-100:JanCode:LOT1234::2025-12-10::1111'
--where Id='1F992F08-788F-405F-3772-08DCFD74456B'


--update wms.WarehouseShipments set [Status]=3, DHLPickup =-1,ShippingBoxesId=null,ShippingBoxesName=null where ShipmentNo in ('WHS000046','WHS000031')
--update wms.WarehouseShipmentLines set [Status]=3,packedqty=0,PackedDate=null where ShipmentNo in ('WHS000031','WHS000046')
--update wms.WarehouseShipments set TrackingNo ='000005' where ShipmentNo='WHS000125'
--update dbo.OrderDispatches set DeliveryCompanyCode='DHL' where id =2747

--delete wms.WarehouseShipmentLines where id='F5C7FF7A-340A-4FE5-9842-2D6756D2A96A'

select * from dbo.Tasks
select * from wms.imagestorage where ResourcesId = '05fd1ea5-ffb3-4fb9-a60a-76c846d31def' and Type =2
exec [sp_getImageStorageByLiatResources] @resourceId = ',bcce56d2-e391-46ab-987e-dfa9b43402e9,',@type=2
--truncate table wms.imagestorage

--delete wms.WarehouseTrans where id='43451271-B2B7-438C-9657-08DD04B9A158'

--update wms.WarehouseTrans set LotNo ='LOT2' where id='53EF917F-DDBE-4E1C-8340-08DD021DC85A'

SELECT *
  FROM [FBT_DEV2].[wms].[Batches]
  order  by CreateAt desc

SELECT *
  FROM [FBT_DEV2].[wms].[Bins]
  order  by CreateAt desc


SELECT ProductCode
	,sum(Qty) InventoryStock
FROM [FBT_DEV2].[wms].WarehouseTrans
where 
	DatePhysical is not null
	--and ProductCode='CLOTH-001'
	--and (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
group by ProductCode

 ;with cteStock as (
	SELECT ProductCode
		,TenantId
		,LotNo
		,[Location]
		,Bin
		,sum(Qty) InventoryStock
	FROM [FBT_DEV2].[wms].WarehouseTrans
	where 
		DatePhysical is not null
		--and ProductCode='A_TestInventory1'
		--and (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
	group by ProductCode,LotNo,[Location],Bin,TenantId
 )
 ,cteOnOrder as (
	SELECT ProductCode
		,TenantId
		,LotNo
		,[Location]
		,Bin
		,sum(Qty) OnOrder
	FROM [FBT_DEV2].[wms].WarehouseTrans
	where 
		DatePhysical is null
		and TransType = 1 --Shipment
	group by ProductCode,LotNo,[Location],Bin,TenantId
 )

 select _cteStock.ProductCode
		,_cteStock.TenantId
		,_cteStock.LotNo
		,_location.LocationName
		,_cteStock.Bin BinCode
		,_batches.ExpirationDate Expired
		,_cteStock.InventoryStock
		,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
		,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
		,_cteOnOrder.[Location]
		,_pro.*
 from cteStock _cteStock
	left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
	left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
	left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
	left join wms.Locations _location on _location.Id = _cteStock.[Location]
where _cteStock.ProductCode='A_TestInventory1'