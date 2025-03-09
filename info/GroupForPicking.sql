-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_GetDataForCreatePicking
	-- Add the parameters for the stored procedure here
	@shipmentId nvarchar(max) = null--list product code.
	,@companyId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare	@shipmentId nvarchar(max) = null--list product code.
	declare	@shipmentId1 nvarchar(max) = null--list product code.
	SET @shipmentId1='WHS000147'
	declare @indexNum int =0;

	DECLARE @ShipmentIdList TABLE (ShipmentId nvarchar(100));
			INSERT INTO @ShipmentIdList
			SELECT value 
			FROM STRING_SPLIT(@shipmentId, ',')
			WHERE value IS NOT NULL;

    -- Insert statements for procedure here
	with cteSortByShimpnentNoAndBin as (
		select 
			--_shipment.ShipmentNo
			--,_shipment.TrackingNo
			_shipment.Id ShipmentId
			,_shipment.SalesNo
			,_shipment.TrackingNo
			,_shipment.TenantId
			,_shipment.TenantName
			,_shipment.ShippingCarrierCode
			,_bin.Id BinId
			,_bin.BinCode
			,_bin.SortOrderNum
			,_shipmentLine.*
		from wms.WarehouseShipmentLines _shipmentLine
			left join wms.WarehouseShipments _shipment on _shipmentLine.ShipmentNo = _shipment.ShipmentNo
			left join wms.Bins _bin on _bin.BinCode = _shipmentLine.Bin
		--where Id in  (SELECT ShipmentId FROM @ShipmentIdList)
		where _shipment.ShipmentNo in ('WHS000154','WHS000155')
			and _shipment.Status = 1 --Open
		--order by _shipment.ShipmentNo,_bin.SortOrderNum asc
	),
	cteGroup as(
		 select _sort.ShipmentNo
			, 'S' + STRING_AGG(CAST(SortOrderNum AS NVARCHAR), '') WITHIN GROUP (ORDER BY SortOrderNum) AS BinJontNumber --SQL Server 2017+

		 from cteSortByShimpnentNoAndBin _sort
		 group by _sort.ShipmentNo
	),
	cteGroupAddIndex as(
		select 
			* 
			--,ROW_NUMBER() OVER (PARTITION BY ShipmentNo ORDER BY BinJontNumber) AS SortedNum --will reset for each ShipmentNo
			,ROW_NUMBER() OVER (ORDER BY ShipmentNo) AS SortedNum -- Now, SortedNum will be 1,2...
		from cteGroup
	)

	select * from cteGroupAddIndex

	select 
		_cteGroup.BinJontNumber
		--,_cteGroup.SortedNum
		,ROW_NUMBER() OVER (PARTITION BY _cte.ShipmentNo ORDER BY _bin.SortOrderNum) AS SortedNum -- Thêm index
		,_cteSort.*
	from cteSortByShimpnentNoAndBin _cteSort
		left join cteGroupAddIndex _cteGroup on _cteGroup.ShipmentNo = _cteSort.ShipmentNo
	order by ShipmentNo,SortOrderNum
		--left join wms.WarehouseShipments _shipment on _shipment.Id = _cte.Id
END
GO

select * from wms.Locations where id ='ca2d4081-1c4d-4377-93e2-97c836e0e18e'
select SortOrderNum, * from wms.Bins where LocationId ='ca2d4081-1c4d-4377-93e2-97c836e0e18e'


select * from wms.WarehouseShipments where ShipmentNo in ('WHS000154','WHS000155')
select * from wms.WarehouseShipmentLines where ShipmentNo in ('WHS000154','WHS000155')