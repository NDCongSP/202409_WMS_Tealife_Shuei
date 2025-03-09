select TrackingNo,LabelFilePath,LabelFileExtension, * from OrderDispatches 
--where labelfilepath is not null or labelfileextension is not null
order by CreateAt desc

update OrderDispatches set TrackingNo ='000005',LabelFilePath = 'C:\FBT-TASK\Labels\KokoroCare\DHL\\D10000000080_3662266484.pdf',LabelFileExtension = 'C:\FBT-TASK\Labels\KokoroCare\DHL\\D10000000080_3662266484.pdf'
where id=2748 and CreateOperatorId = 'WMS'

select * from wms.ShippingCarriers
select * from wms.ShippingBoxes 
where ShippingCarrierCode='日本郵便'
	and BoxName = '無地箱'
order by BoxType asc


select * from Products where ProductCode ='ELEC-001'
select * from ProductJanCodes where ProductId = 3


select * from wms.WarehouseReceiptOrderLine

select * from wms.WarehouseReceiptOrder
select * from wms.WarehousePutAwaysO
select * from wms.WarehousePutAwayLines

select * from wms.Locations

--update wms.WarehousePutAways set [Location] = 'a5c0f5ea-6e46-460d-8e7f-09b591439eee' where [Location] is null	
 in ('366f2dfa-8721-4088-948c-d9c91fdffd5c','8e161d48-1811-4370-b529-202fed4d6797','f0aa53f6-c736-4e6f-b51a-33813f85d2a9')

 select * from fbt_dev2.wms.WarehousePutAwayLines where PutAwayNo = 'WHS000007'
 select * from fbt_dev2..Products where ProductCode='CLOTH-001'
 --https://kokorocares.com/products/bekko-ame-tortoiseshell-hard-candy?_pos=1&_sid=39273a74e&_ss=r

 --update products set ProductURL='https://kokorocares.com/products/bekko-ame-tortoiseshell-hard-candy?_pos=1&_sid=39273a74e&_ss=r' where ProductCode='CLOTH-001'