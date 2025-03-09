
select * from Products where ProductCode in ('A_TestInventory1','A_TestInventory2')

select * from Products where CompanyId =2


select * from wms.WarehouseReceiptOrder
where ReceiptNo in ('WHR000387','WHR000388','WHR000389')
select * from wms.WarehouseReceiptOrderLine 
where ProductCode in ('A_TestInventory1','A_TestInventory2')

select * from wms.WarehouseShipments
where ShipmentNo in ('WHS000124')
select * from wms.WarehouseShipmentLines 
where ProductCode in ('A_TestInventory1','A_TestInventory2')

--update wms.WarehouseShipments set TrackingNo='000005' where Id='73172810-B57B-452E-270D-08DD49E90297'

SELECT ProductCode
	,LotNo
			,[Location]
			,Bin
			,TenantId
	,sum(Qty) InventoryStock
FROM [FBT_DEV2].[wms].WarehouseTrans
where 
	DatePhysical is not null
	and ProductCode in ('A_TestInventory1','A_TestInventory2')
	--and (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
group by ProductCode,LotNo,[Location],Bin,TenantId
	

	select * from wms.WarehouseTrans 
where ProductCode in ('A_TestInventory1','A_TestInventory2')
order by CreateAt desc

select * from view_wmsGetInventoryInfoFlowBinLotTenant where ProductCode in ('A_TestInventory1','A_TestInventory2')
exec [sp_getInventoryInfoFlowBinLot] ',A_TestInventory1,A_TestInventory2,',0

	;with cteStock as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) InventoryStock
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is not null
			and ProductCode in ('A_TestInventory1','A_TestInventory2')
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )
	 ,cteOnOrder as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) OnOrder
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is null
			and TransType = 1 --Shipment
			and ProductCode in ('A_TestInventory1','A_TestInventory2')
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )

	 select _cteStock.LotNo
			,_location.LocationName
			,_cteStock.Bin BinCode
			,_batches.ExpirationDate Expired
			,_cteStock.InventoryStock
			,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
			,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
			,_cteStock.[Location]
			,_cteStock.TenantId
			,_comp.FullName
			,_pro.*
	 from cteStock _cteStock
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
			and _cteStock.TenantId=_cteOnOrder.TenantId
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
			and _cteStock.TenantId=_pro.CompanyId
		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
			and _cteStock.TenantId=_batches.TenantId
		left join wms.Locations _location on _location.Id = _cteStock.[Location] 
		left join Companies _comp on _comp.AuthPTenantId = _cteStock.TenantId
	where _cteStock.ProductCode	in ('A_TestInventory1','A_TestInventory2')

	exec [sp_productGetDataMaster]

;with cteStock as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) InventoryStock
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is not null
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )
	 ,cteOnOrder as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) OnOrder
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is null
			and TransType = 1 --Shipment
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )

	 select _cteStock.LotNo
			,_location.LocationName
			,_cteStock.Bin BinCode
			,_batches.ExpirationDate Expired
			,_cteStock.InventoryStock
			,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
			,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
			,_cteOnOrder.[Location]
			,_cteStock.TenantId
			,_pro.*
	 from cteStock _cteStock
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
		left join wms.Locations _location on _location.Id = TRY_CONVERT(uniqueidentifier, _cteStock.Location)