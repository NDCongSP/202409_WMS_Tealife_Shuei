SELECT
	TRN.ProductCode
	,TRN.Qty
	,TRN.DatePhysical
	,TRN.TransType
	,TRN.TransNumber
	,TRN.Bin
	,TRN.LotNo
	,RODR.TransQty
	,RODR.ExpirationDate as 'ExpirationDate(ì¸)'
	,PAWY.TransQty
	,PAWY.ExpirationDate as 'ExpirationDate(íI)'
	,PICK.ActualQty
	,PICK.ExpirationDate as 'ExpirationDate(Pick)'
	,SHP.ShipmentQty
	,SHP.ExpirationDate as 'ExpirationDate(èo)'
	,ADJ.Qty
	,ADJ.ExpirationDate as 'ExpirationDate(èo)'
FROM [wms].[WarehouseTrans] TRN
left outer join [wms].[WarehouseReceiptOrderLine] RODR
on TRN.TransNumber=RODR.ReceiptNo and TRN.ProductCode=RODR.ProductCode
left outer join [wms].[WarehousePutAwayLines] PAWY
on TRN.TransNumber=PAWY.PutAwayNo and TRN.ProductCode=PAWY.ProductCode
left outer join [wms].[WarehousePickingLines] PICK
on TRN.TransNumber=PICK.PickNo and TRN.ProductCode=PICK.ProductCode
left outer join [wms].[WarehouseShipmentLines] SHP
on TRN.TransNumber=SHP.ShipmentNo and TRN.ProductCode=SHP.ProductCode
left outer join [wms].[InventAdjustmentLines] ADJ
on TRN.TransNumber=ADJ.AdjustmentNo and TRN.ProductCode=ADJ.ProductCode
where 0=0
and TRN.ProductCode='SISS026'
order by TRN.CreateAt,TRN.Qty
;

select
*
FROM [dbo].[view_wmsGetInventoryInfoFlowBinLot]
where ProductCode='SISS026'
;
