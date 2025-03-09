select
		--ODR.OrderId as CustomerManagementNumber--'1:お客様側管理番号'
		ODD.DeliveryId as CustomerManagementNumber--'1:お客様側管理番号'
		,'20250221' as ScheduledShippingDate--2:発送予定日
		,'' as ShippingTimeCategory--'3:発送予定時間区分'
		,'' as ShippingDeadline--'4:出荷期限日'
		,'' as ArrivalDeadline--'5:到着期限日'
		,'0' as MailType--'6:郵便種別'
		,'0' as RefrigerationType--'7:保冷種別'
		,'0' as PaymentType--8:元／着払／代引  
		,'0' as SecurityRecordType--'9:書留／セキュリティ／特定記録種別'
		,'1100780 ' as InvoiceType--'10:送り状種別'
		,'' as DeliveryAddressCode--11:お届け先 コード
		,ODR.DeliveryZipcode as DeliveryAddressPostalCode--12:お届け先 郵便番号
		,ODR.DeliveryState as DeliveryAddress1--13:お届け先 住所１
		,ODR.DeliveryCity as DeliveryAddress2--14:お届け先 住所2
		,ODR.DeliveryAddress1 + isnull(ODR.DeliveryAddress2,'') as DeliveryAddress3--15:お届け先 住所3
		,ODR.DeliveryName as DeliveryAddressName1--16:お届け先 名称１
		,'' as DeliveryAddressName2--17:お届け先 名称2
		,'0' as DeliveryAddressTitleType--18:お届け先 敬称区分
		,replace(ODR.DeliveryPhone,'+8180','080') as DeliveryAddressTelephoneNumber--19:お届け先 電話番号
		,ODR.DeliveryMail as DeliveryAddressEmailAddress1--20:お届け先 メールアドレス１
		,'0' as DeliveryAddressOfficeRetainedCategory--21:お届け先 局留め区分
		,'' as DeliveryAddressPostOfficeName--22:お届け先 局留め郵便局名
		,'' as DeliveryAddressPostalCode1--23:お届け先 局留め郵便番号
		,'0' as DeliveryAddressOfficeMailUsageCategory--24:お届け先 局留めメール利用区分
		,'0' as DeliveryAddressDeliveryNotificationEmailUsageCategory--25:お届け先 配達予告メール利用区分
		,'0' as DeliveryAddressReDeliveryNoticeEmailUsageCategory--26:お届け先 再配達予告メール利用区分 
		,'' as RequesterCode--27:ご依頼主 コード
		,'436-0082' as RequesterPostalCode--28:ご依頼主 郵便番号
		,'静岡県掛川市' as RequesterAddress1--29:ご依頼主 住所１
		,'淡陽' as RequesterAddress2--30:ご依頼主 住所2
		,'18番1' as RequesterAddress3--31:ご依頼主 住所3
		,'ティーライフ株式会社' as RequesterName1--32:ご依頼主 名称１
		,'' as RequesterName2--33:ご依頼主 名称2
		,'0' as RequesterTitle--34:ご依頼主 敬称
		,'0547462232' as RequesterPhoneNumber--35:ご依頼主 電話番号
		,'' as RequesterEmailAddress1-- 36:ご依頼主 メールアドレス１
		,'0' as RequesterDeliveryNotificationEmailUsageCategory--37:ご依頼主 お届け通知メール利用区分
		,'0' as RequesterDeliveryNotificationPostcardUsageCategory--38:ご依頼主 お届け通知はがき使用区分
		,'' as OrderNumber--39:受注番号
		,'' as OrderDate--40:注文日
		,'1' as BreakdownCategory--41:こわれもの区分
		,'0' as RawFoodCategory--42:なまもの区分
		,'1' as BinClassification--43:ビン類区分
		,'1' as UpsideDownProhibitedCategory--44:逆さま厳禁区分
		,'0' as UnderpinningProhibitedCategory--45:下積み厳禁区分
		,'80' as ProductSizeWeightCategory--46:商品サイズ／重さ区分
		,PRD.Weight as TotalWeight_G--47:重量合計（ｇ）
		,'' as RegisteredMailDamagesRequired--48:書留　損害要償額
		,'0' as ExpressDelivery_DeliveryDateSpecifiedType--49:速達・配達日指定種別
		,'' as DeliveryDate_DesiredDate--50:配達指定日／希望日
		,'00' as DeliveryTimeCategory--51:配達時間帯区分
		,'0' as Yu_PackMultipleDiscount--52:ゆうパック複数個割引
		,'0' as Yu_PackSameDiscount--53:ゆうパック同一割引
		,'' as MultiplePackages--54:複数個口数
		,'食品詰め合わせ' as ArticleName1--55:記事名１
		,'' as ArticleName2--56:記事名２
		,'' as FreeItem01--57:フリー項目０１
		,'' as FreeItem02--58:フリー項目０２
		,'' as FreeItem03--59:フリー項目０３
		,'' as FreeItem04--60:フリー項目０４
		,'' as FreeItem05--61:フリー項目０５
		,'' as FreeItem06--62:フリー項目０６
		,'' as FreeItem07--63:フリー項目０７
		,'' as FreeItem08--64:フリー項目０８
		,'' as FreeItem09--65:フリー項目０９
		,'' as FreeItem10--66:フリー項目１０
		,'' as CashOnDelivery_Cod_Amount--67:代引金額
		,'' as CashOnDelivery_Cod_TaxAmount--68:代引消費税金額
		,'食品詰め合わせ' as ProductName--69:品名
	from Orders ODR
		inner join OrderDispatches ODP
			on ODR.OrderId=ODP.OrderId
			and ODP.IsDeleted='0'
		inner join OrderDispatchProducts ODD
			on ODP.OrderId=ODD.OrderId
			and ODP.DeliveryId=ODD.DeliveryId
			and ODD.IsDeleted='0'
		inner join Products PRD
			on ODD.ProductCode=PRD.ProductCode
	where 0=0
	--and ODR.OrderStatus in('30')
	--and ODR.OrderStatus not in('40','99')
	and ODR.IsDeleted='0'
	and ODP.DeliveryCompanyCode in('JP-YP')
	order by ODD.ProductCode,ODR.OrderName