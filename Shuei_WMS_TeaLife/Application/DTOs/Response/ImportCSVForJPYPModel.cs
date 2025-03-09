using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Response
{
    public class ImportCSVForJPYPModel
    {
        [Display(Name = "お客様側管理番号")]
        //[Display(Name = "a")]
        public string? CustomerManagementNumber { get; set; }
        [Display(Name = "お問い合わせ番号")]
        //[Display(Name = "b")]
        public string? InquiryNumber { get; set; }
        [Display(Name = "代表お問い合わせ番号")]
        //[Display(Name = "c")]
        public string? RepresentativeInquiryNumber { get; set; }
    }
}
