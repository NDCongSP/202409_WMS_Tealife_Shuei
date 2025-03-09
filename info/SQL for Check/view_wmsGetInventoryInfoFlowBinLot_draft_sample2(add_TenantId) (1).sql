	with cteStock as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) InventoryStock
		FROM [FBT].[wms].WarehouseTrans
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
		FROM [FBT].[wms].WarehouseTrans
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
		--left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
		--left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
		--left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
		--left join wms.Locations _location on _location.Id = _cteStock.[Location] 
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
			and _cteStock.TenantId=_cteOnOrder.TenantId
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
			and _cteStock.TenantId=_pro.CompanyId
		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
			and _cteStock.TenantId=_batches.TenantId
		left join wms.Locations _location on _location.Id = _cteStock.[Location] 