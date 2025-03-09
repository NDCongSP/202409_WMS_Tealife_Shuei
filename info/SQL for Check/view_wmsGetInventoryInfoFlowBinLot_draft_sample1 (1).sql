	with cteStock as (
		SELECT
			SMR.ProductCode
			,SMR.LotNo
			,SMR.Location
			,SMR.Bin
			,SMR.ExpirationDate
			,SMR.TenantId
			,sum(SMR.Qty) InventoryStock
		FROM(
			SELECT
				TRN.ProductCode
				,TRN.Qty
				,TRN.DatePhysical
				,TRN.TransType
				,TRN.TransNumber
				,TRN.Bin
				,TRN.LotNo
				,TRN.Location
				,TRN.TenantId
				,CASE
				  WHEN RODR.ExpirationDate IS NOT NULL THEN RODR.ExpirationDate
				  WHEN PAWY.ExpirationDate IS NOT NULL THEN PAWY.ExpirationDate
				  WHEN PICK.ExpirationDate IS NOT NULL THEN PICK.ExpirationDate
				  WHEN SHIP.ExpirationDate IS NOT NULL THEN SHIP.ExpirationDate
				  WHEN ADJT.ExpirationDate IS NOT NULL THEN ADJT.ExpirationDate
				  ELSE NULL
				 END AS 'ExpirationDate'
			FROM [wms].[WarehouseTrans] TRN
			left outer join [wms].[WarehouseReceiptOrderLine] RODR
			on TRN.TransNumber=RODR.ReceiptNo and TRN.ProductCode=RODR.ProductCode
			left outer join [wms].[WarehousePutAwayLines] PAWY
			on TRN.TransNumber=PAWY.PutAwayNo and TRN.ProductCode=PAWY.ProductCode
			left outer join [wms].[WarehousePickingLines] PICK
			on TRN.TransNumber=PICK.PickNo and TRN.ProductCode=PICK.ProductCode
			left outer join [wms].[WarehouseShipmentLines] SHIP
			on TRN.TransNumber=SHIP.ShipmentNo and TRN.ProductCode=SHIP.ProductCode
			left outer join [wms].[InventAdjustmentLines] ADJT
			on TRN.TransNumber=ADJT.AdjustmentNo and TRN.ProductCode=ADJT.ProductCode
			where 0=0
			AND TRN.DatePhysical is not null
		) AS SMR
		group by SMR.ProductCode,SMR.LotNo,SMR.[Location],SMR.Bin,SMR.TenantId,SMR.ExpirationDate
	 )
	 ,cteOnOrder as (
		SELECT
			SMR.ProductCode
			,SMR.LotNo
			,SMR.Location
			,SMR.Bin
			,SMR.ExpirationDate
			,SMR.TenantId
			,sum(SMR.Qty) OnOrder
		FROM(
			SELECT
				TRN.ProductCode
				,TRN.Qty
				,TRN.DatePhysical
				,TRN.TransType
				,TRN.TransNumber
				,TRN.Bin
				,TRN.LotNo
				,TRN.Location
				,TRN.TenantId
				,CASE
				  WHEN RODR.ExpirationDate IS NOT NULL THEN RODR.ExpirationDate
				  WHEN PAWY.ExpirationDate IS NOT NULL THEN PAWY.ExpirationDate
				  WHEN PICK.ExpirationDate IS NOT NULL THEN PICK.ExpirationDate
				  WHEN SHIP.ExpirationDate IS NOT NULL THEN SHIP.ExpirationDate
				  WHEN ADJT.ExpirationDate IS NOT NULL THEN ADJT.ExpirationDate
				  ELSE NULL
				 END AS 'ExpirationDate'
			FROM [wms].[WarehouseTrans] TRN
			left outer join [wms].[WarehouseReceiptOrderLine] RODR
			on TRN.TransNumber=RODR.ReceiptNo and TRN.ProductCode=RODR.ProductCode
			left outer join [wms].[WarehousePutAwayLines] PAWY
			on TRN.TransNumber=PAWY.PutAwayNo and TRN.ProductCode=PAWY.ProductCode
			left outer join [wms].[WarehousePickingLines] PICK
			on TRN.TransNumber=PICK.PickNo and TRN.ProductCode=PICK.ProductCode
			left outer join [wms].[WarehouseShipmentLines] SHIP
			on TRN.TransNumber=SHIP.ShipmentNo and TRN.ProductCode=SHIP.ProductCode
			left outer join [wms].[InventAdjustmentLines] ADJT
			on TRN.TransNumber=ADJT.AdjustmentNo and TRN.ProductCode=ADJT.ProductCode
			where 0=0
			AND TRN.DatePhysical is null
			AND TRN.TransType = 1 --Shipment
		) AS SMR
		group by SMR.ProductCode,SMR.LotNo,SMR.[Location],SMR.Bin,SMR.TenantId,SMR.ExpirationDate
	 )

	 select _cteStock.LotNo
			,_location.LocationName
			,_cteStock.Bin BinCode
			--,_batches.ExpirationDate Expired
			,convert(varchar,_cteStock.ExpirationDate,23) Expired
			,_cteStock.InventoryStock
			,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
			,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
			,_cteOnOrder.[Location]
			,_cteStock.TenantId
			,_pro.*
	 from cteStock _cteStock
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
			and _cteOnOrder.ExpirationDate = _cteStock.ExpirationDate
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
--		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
		left join wms.Locations _location on _location.Id = TRY_CONVERT(uniqueidentifier, _cteStock.Location)

