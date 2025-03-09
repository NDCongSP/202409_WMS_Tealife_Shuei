using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Response
{
    public class ShippingInfoCSV
    {
        [Display(Name = "1:お客様側管理番号")]
        public string? CustomerManagementNumber { get; set; }

        [Display(Name = "2:発送予定日")]
        public DateTime? ScheduledShippingDate { get; set; }

        [Display(Name = "3:発送予定時間区分")]
        public string? ShippingTimeCategory { get; set; } = string.Empty;

        [Display(Name = "4:出荷期限日")]
        public DateTime? ShippingDeadline { get; set; } = null;

        [Display(Name = "5:到着期限日")]
        public DateTime? ArrivalDeadline { get; set; } = null;

        [Display(Name = "6:郵便種別")]
        public string? MailType { get; set; } = "0";

        [Display(Name = "7:保冷種別")]
        public string? RefrigerationType { get; set; } = "0";

        /// <summary>
        /// Original/PaymentToDelivery/CashOnDelivery(Cod).
        /// </summary>
        [Display(Name = "8:元／着払／代引")]
        public string? PaymentType { get; set; } = "0";

        /// <summary>
        /// Registered/Security/SpecialRecordType--9:書留／セキュリティ／特定記録種別
        /// </summary>
        [Display(Name = "9:書留／セキュリティ／特定記録種別")]
        public string? SecurityRecordType { get; set; } = "0";

        [Display(Name = "10:送り状種別")]
        public string? InvoiceType { get; set; } = "1100780";

        [Display(Name = "11:お届け先 コード")]
        public string? DeliveryAddressCode { get; set; } = string.Empty;

        [Display(Name = "12:お届け先 郵便番号")]
        public string? DeliveryAddressPostalCode { get; set; }

        [Display(Name = "13:お届け先 住所１")]
        public string? DeliveryAddress1 { get; set; }

        [Display(Name = "14:お届け先 住所２")]
        public string? DeliveryAddress2 { get; set; }

        [Display(Name = "15:お届け先 住所３")]
        public string? DeliveryAddress3 { get; set; }

        [Display(Name = "16:お届け先 名称１")]
        public string? DeliveryAddressName1 { get; set; }

        [Display(Name = "17:お届け先 名称２")]
        public string? DeliveryAddressName2 { get; set; } = string.Empty;

        [Display(Name = "18:お届け先 敬称区分")]
        public string? DeliveryAddressTitleType { get; set; } = "0";

        [Display(Name = "19:お届け先 電話番号")]
        public string? DeliveryAddressTelephoneNumber { get; set; }

        [Display(Name = "20:お届け先 メールアドレス１")]
        public string? DeliveryAddressEmailAddress1 { get; set; }

        [Display(Name = "21:お届け先 局留め区分")]
        public string? DeliveryAddressOfficeRetainedCategory { get; set; } = "0";

        [Display(Name = "22:お届け先 局留め郵便局名")]
        public string? DeliveryAddressPostOfficeName { get; set; } = string.Empty;

        [Display(Name = "23:お届け先 局留め郵便番号")]
        public string? DeliveryAddressPostalCode1 { get; set; } = string.Empty;

        [Display(Name = "24:お届け先 局留めメール利用区分")]
        public string? DeliveryAddressOfficeMailUsageCategory { get; set; } = "0";

        [Display(Name = "25:お届け先 配達予告メール利用区分")]
        public string? DeliveryAddressDeliveryNotificationEmailUsageCategory { get; set; } = "0";

        [Display(Name = "26:お届け先 再配達予告メール利用区分")]
        public string? DeliveryAddressReDeliveryNoticeEmailUsageCategory { get; set; } = "0";

        [Display(Name = "27:ご依頼主 コード")]
        public string? RequesterCode { get; set; } = string.Empty;

        [Display(Name = "28:ご依頼主 郵便番号")]
        public string? RequesterPostalCode { get; set; } = "436-0082";

        [Display(Name = "29:ご依頼主 住所１")]
        public string? RequesterAddress1 { get; set; } = "静岡県掛川市";

        [Display(Name = "30:ご依頼主 住所２")]
        public string? RequesterAddress2 { get; set; } = "淡陽";

        [Display(Name = "31:ご依頼主 住所３")]
        public string? RequesterAddress3 { get; set; } = "18番1";

        [Display(Name = "32:ご依頼主 名称１")]
        public string? RequesterName1 { get; set; } = "ティーライフ株式会社";

        [Display(Name = "33:ご依頼主 名称２")]
        public string? RequesterName2 { get; set; } = string.Empty;

        [Display(Name = "34:ご依頼主 敬称")]
        public string? RequesterTitle { get; set; } = "0";

        [Display(Name = "35:ご依頼主 電話番号")]
        public string? RequesterPhoneNumber { get; set; } = "0547462232";

        [Display(Name = "36:ご依頼主 メールアドレス１")]
        public string? RequesterEmailAddress1 { get; set; } = string.Empty;

        [Display(Name = "37:ご依頼主 お届け通知メール利用区分")]
        public string? RequesterDeliveryNotificationEmailUsageCategory { get; set; } = "0";

        [Display(Name = "38:ご依頼主 お届け通知はがき使用区分")]
        public string? RequesterDeliveryNotificationPostcardUsageCategory { get; set; } = "0";

        [Display(Name = "39:受注番号")]
        public string? OrderNumber { get; set; } = string.Empty;

        [Display(Name = "40:注文日")]
        public DateTime? OrderDate { get; set; } = null;

        [Display(Name = "41:こわれもの区分")]
        public bool? BreakdownCategory { get; set; } = true;

        [Display(Name = "42:なまもの区分")]
        public bool? RawFoodCategory { get; set; } = false;

        [Display(Name = "43:ビン類区分")]
        public bool? BinClassification { get; set; } = true;

        [Display(Name = "44:逆さま厳禁区分")]
        public bool? UpsideDownProhibitedCategory { get; set; } = true;

        [Display(Name = "45:下積み厳禁区分")]
        public bool? UnderpinningProhibitedCategory { get; set; } = false;

        [Display(Name = "46:商品サイズ／重さ区分")]
        public string? ProductSizeWeightCategory { get; set; } = "80";

        [Display(Name = "47:重量合計（ｇ）")]
        public int? TotalWeight_G { get; set; }

        [Display(Name = "48:書留 損害要償額")]
        public decimal? RegisteredMailDamagesRequired { get; set; }

        [Display(Name = "49:速達・配達日指定種別")]
        public string? ExpressDelivery_DeliveryDateSpecifiedType { get; set; } = "0";

        [Display(Name = "50:配達指定日／希望日")]
        public DateTime? DeliveryDate_DesiredDate { get; set; } = null;

        [Display(Name = "51:配達時間帯区分")]
        public string? DeliveryTimeCategory { get; set; } = "00";

        [Display(Name = "52:ゆうパック複数個割引")]
        public string? Yu_PackMultipleDiscount { get; set; } = "0";

        [Display(Name = "53:ゆうパック同一割引")]
        public string? Yu_PackSameDiscount { get; set; } = "0";

        [Display(Name = "54:複数個口数")]
        public int? MultiplePackages { get; set; }

        [Display(Name = "55:記事名１")]
        public string? ArticleName1 { get; set; } = "食品詰め合わせ";

        [Display(Name = "56:記事名２")]
        public string? ArticleName2 { get; set; } = string.Empty;

        [Display(Name = "57:フリー項目０１")]
        public string? FreeItem01 { get; set; } = string.Empty;

        [Display(Name = "58:フリー項目０２")]
        public string? FreeItem02 { get; set; }= string.Empty;

        [Display(Name = "59:フリー項目０３")]
        public string? FreeItem03 { get; set; }= string.Empty;

        [Display(Name = "60:フリー項目０４")]
        public string? FreeItem04 { get; set; }= string.Empty;

        [Display(Name = "61:フリー項目０５")]
        public string? FreeItem05 { get; set; }= string.Empty;
        [Display(Name = "62:フリー項目０６")]
        public string? FreeItem06 { get; set; }= string.Empty;
        [Display(Name = "63:フリー項目０７")]
        public string? FreeItem07 { get; set; }= string.Empty;
        [Display(Name = "64:フリー項目０８")]
        public string? FreeItem08 { get; set; }= string.Empty;
        [Display(Name = "65:フリー項目０９")]
        public string? FreeItem09 { get; set; }= string.Empty;
        [Display(Name = "66:フリー項目１０")]
        public string? FreeItem10 { get; set; }= string.Empty;

        [Display(Name = "67:代引金額")]
        public decimal? CashOnDelivery_Cod_Amount { get; set; }

        [Display(Name = "68:代引消費税金額")]
        public decimal? CashOnDelivery_Cod_TaxAmount { get; set; }

        [Display(Name = "69:品名")]
        public string? ProductName { get; set; } = "食品詰め合わせ";
    }

}
