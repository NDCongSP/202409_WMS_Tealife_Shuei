select top 10 * from wms.InventTransfer	
--where TransferNo in ('WHT000002','WHT000013','WHT000016','WHT000006','WHT000018') 
order by CreateAt desc

select top 10 * from wms.InventTransferLines 
--where TransferNo='WHT000013' 
order by CreateAt desc


select top 10 * from wms.WarehouseTrans 
--where TransferNo='WHT000013' 
order by CreateAt desc

--select * from Products where ProductCode='KOKORO-T001'

--update wms.InventTransfer set TenantId = 2 	where TransferNo in ('WHT000002','WHT000013','WHT000016','WHT000006')