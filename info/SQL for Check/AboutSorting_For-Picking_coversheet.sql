declare @PlanShipDate varchar(10)
set @PlanShipDate='2025-02-28'

SELECT
 HDR.ShipmentNo
 ,DDL.ProductCode
 ,DDL.Bin
 ,BIN.SortOrderNum
 ,*
FROM [wms].[WarehouseShipments] HDR
inner join [wms].[WarehouseShipmentLines] DDL
on HDR.ShipmentNo=DDL.ShipmentNo
inner join [wms].[Bins] BIN
on DDL.Bin=BIN.BinCode
where 0=0
and HDR.Status='1'
and HDR.PlanShipDate=@PlanShipDate
Order by HDR.ShipmentNo
/*
*/

/**********************************************************
--sample

SELECT
    a.TEAM AS TEAM,
    (
	 SELECT ',' + MEMBER FROM Table1 b 
	 WHERE b.TEAM = a.TEAM 
	 ORDER BY b.NO FOR XML PATH('')
	) AS MEMBER
FROM
    Table1 a
**********************************************************/

SELECT
	HDR2.ShippingCarrierCode
	,HDR2.ShipmentNo
	,'S' + 
--	STUFF(
		(SELECT
--			'-'
			+ cast(BIN.SortOrderNum as varchar)
		FROM [wms].[WarehouseShipments] HDR
		inner join [wms].[WarehouseShipmentLines] DDL
		on HDR.ShipmentNo=DDL.ShipmentNo
		inner join [wms].[Bins] BIN
		on DDL.Bin=BIN.BinCode
		where 0=0
		and HDR.Status='1'
		and HDR.PlanShipDate=@PlanShipDate
		and HDR.ShipmentNo = HDR2.ShipmentNo
		Order by HDR.ShipmentNo,BIN.SortOrderNum
		FOR XML PATH('')
--		), 1, 1, ''
) as BINSortNum
FROM [wms].[WarehouseShipments] HDR2
where 0=0
and HDR2.Status='1'
and HDR2.PlanShipDate=@PlanShipDate
order by HDR2.ShippingCarrierCode,BINSortNum,HDR2.ShipmentNo
;
