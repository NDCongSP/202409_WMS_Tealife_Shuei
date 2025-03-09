select TenantId,TransDate
from wms.WarehousePutAways
group by TenantId,TransDate

select * from wms.WarehousePutAways
where TenantId = 1 and TransDate='2024-10-29'

select * from wms.WarehousePutAwayLines where PutAwayNo in ('WHP000006','WHP000011','WHP000012')

exec sp_dashboardGetPutawayData

--	SELECT 
--    wpa.TenantId, 
--    wpa.TransDate, 
--    --SUM(wpl.JournalQty) AS TotalJournalQty
--	COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
--	COALESCE(SUM(wpl.TransQty), 0)  AS TotalTransQty
--FROM wms.WarehousePutAways wpa
--JOIN wms.WarehousePutAwayLines wpl 
--    ON wpa.PutAwayNo = wpl.PutAwayNo
--GROUP BY wpa.TenantId, wpa.TransDate
--ORDER BY wpa.TenantId, wpa.TransDate;

;with cteGetData as (
	SELECT 
		wpa.TenantId, 
		wpa.TransDate, 
		--SUM(wpl.JournalQty) AS TotalJournalQty
		COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
		COALESCE(SUM(wpl.TransQty), 0)  AS TotalTransQty,
		COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
	FROM wms.WarehousePutAways wpa
	JOIN wms.WarehousePutAwayLines wpl 
		ON wpa.PutAwayNo = wpl.PutAwayNo
	GROUP BY wpa.TenantId, wpa.TransDate
	),
	cteGetDataOpen as (
		SELECT 
			wpa.TenantId, 
			wpa.TransDate, 
			--SUM(wpl.JournalQty) AS TotalJournalQty
			COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
			COALESCE(SUM(wpl.TransQty), 0)  AS TotalTransQty,
			COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
		FROM wms.WarehousePutAways wpa
		JOIN wms.WarehousePutAwayLines wpl 
			ON wpa.PutAwayNo = wpl.PutAwayNo
		where wpa.Status = 0
		GROUP BY wpa.TenantId, wpa.TransDate
		),
	cteGetDataComplete as (
		SELECT 
			wpa.TenantId, 
			wpa.TransDate, 
			--SUM(wpl.JournalQty) AS TotalJournalQty
			COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
			COALESCE(SUM(wpl.TransQty), 0)  AS TotalTransQty,
			COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
		FROM wms.WarehousePutAways wpa
		JOIN wms.WarehousePutAwayLines wpl 
			ON wpa.PutAwayNo = wpl.PutAwayNo
		where wpa.Status = 1
		GROUP BY wpa.TenantId, wpa.TransDate
		)

select _cte.TransDate [Period]
	,_com.FullName Tenant
	,CONCAT(_cte.TotalTransQty,'/',_cte.TotalJournalQty) ExpectedStock

	,case
	when _cteOpen.TotalTransQty is not null then CONCAT(_cteOpen.TotalRemainQty,'/',_cteOpen.TotalJournalQty)
	else null
	end RemainingNumber

	--,CONCAT(ROUND(_cte.TotalTransQty / NULLIF(_cte.TotalJournalQty, 0) * 100, 2),'%') ProgressRateString
	,ROUND(_cte.TotalTransQty / NULLIF(_cte.TotalJournalQty, 0) * 100, 2) ProgressRate --float
	,_cte.TotalJournalQty TotalJournalQty --int
	,_cte.TotalTransQty TotalTransQty --int
	,_cte.TotalRemainQty TotalRemainingNumber --int
	,round(_cteComplete.TotalTransQty/8, 1) Productivity --just only putaway completed
	,_cteOpen.TotalJournalQty TotalJournalQtyOpen --int
	,_cteOpen.TotalTransQty TotalTransQtyOpen --int
	,_cteOpen.TotalRemainQty TotalRemainingNumberOpen --int
	,_cteComplete.TotalJournalQty TotalJournalQtyComplete --int
	,_cteComplete.TotalTransQty TotalTransQtyComplete --int
	,_cteComplete.TotalRemainQty TotalRemainingNumberComplete --int
from cteGetData _cte
	left join dbo.Companies _com on _com.AuthPTenantId = _cte.TenantId
	left join cteGetDataOpen _cteOpen on _cteOpen.TenantId = _cte.TenantId and _cteOpen.TransDate = _cte.TransDate
	left join cteGetDataComplete _cteComplete on _cteComplete.TenantId = _cte.TenantId and _cteComplete.TransDate = _cte.TransDate
ORDER BY _cte.TenantId, _cte.TransDate