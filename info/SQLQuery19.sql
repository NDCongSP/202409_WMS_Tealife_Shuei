 SELECT * FROM wms.AspNetRoles
 
 SELECT * FROM wms.[AspNetUsers]
  
  --SELECT * FROM authp.Tenants
  
    SELECT * FROM wms.[Permissions]

  SELECT * FROM wms.AspNetUserRoles
  
    SELECT * FROM wms.[RoleToPermission]

  SELECT * FROM wms.usertotenant

  --truncate table wms.[AspNetUsers]

  SELECT  *  FROM wms.refreshtokens  order by ExpirationTime desc


              SELECT *
  FROM wms.Locations
	  SELECT *
  FROM wms.Bins

          SELECT *
  FROM wms.Units

            SELECT *
  FROM dbo.Suppliers

  select* from WMS.Logtime order by createddate desc
  --truncate table Wms.[Logtime]

--chinh sau data type column
--ALTER TABLE wms.WarehousePackingLines ADD ShipmentLineId uniqueidentifier;
--ALTER TABLE dbo.WarehouseShipments ALTER COLUMN [PlanShipDate] date;

select * from dbo.Products

select * from wms.NumberSequences
select * from wms.Locations
select * from wms.Bins where LocationId='63BD1CD2-803D-44C1-815D-A19DF555E2E1'

select * from wms.ShippingCarriers order by CreateAt desc
select * from wms.ShippingBoxes order by CreateAt desc
select * from wms.Units

select * from dbo.Currencies
select * from authp.Tenants
select * from Suppliers
select * from ProductCategories
select * from Products
select * from ProductJanCodes
  
select * from wms.WarehouseShipments order by CreateAt desc
select * from wms.WarehouseShipmentLines order by CreateAt desc

select * from wms.WarehousePickingList order by CreateAt desc
select * from wms.WarehousePickingLines

--select * from wms.WarehousePackingList order by CreateAt desc
--select * from wms.WarehousePackingLines order by CreateAt desc

select * from wms.WarehouseTrans where TransType =1 order by CreateAt desc

--exec sp_packingListGetDataMaster --@PlanShipDateFrom='2024-10-16',@PlanShipDateTo='2024-10-19'

--update Products set SupplierId =2


--delete wms.WarehouseShipments where id='e06214b3-8087-ef11-9100-917c6e3f8094'
--update wms.WarehouseShipmentLines set Status=5

select * from view_wmsGetInventoryInfo
select * from view_wmsGetInventoryInfoFlowBinLot
exec sp_getinventoryInfo 
exec sp_getinventoryInfo @productCode = 'PRO999'

--exec sp_getInventoryInfoFlowBinLot
exec sp_getInventoryInfoFlowBinLot @productCode = 'PRO999'

exec [sp_productGetDataMaster] @ProductStatus = 2, @id=18

