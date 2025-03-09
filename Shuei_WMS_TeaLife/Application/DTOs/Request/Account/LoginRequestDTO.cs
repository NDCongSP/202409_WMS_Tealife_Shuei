using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Account
{
    public class LoginRequestDTO
    {
        [EmailAddress, Required, DataType(DataType.EmailAddress)]
        [RegularExpression("[^@ \\t\\r\\n]+@[^@ \\t\\r\\n]+\\.[^@ \\t\\r\\n]+",
            ErrorMessage = "Your email is not valid, provide valid email such ass ...@gmail, @hostmail, etc...")]
        [DisplayName("Email Address")] public string EmailAddress { get; set; } = string.Empty;

        //[Required(AllowEmptyStrings = false, ErrorMessage = "User Name is required")]
        //public string? UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required"),DataType(DataType.Password)]
        //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*_]).{8,}$",
        //    ErrorMessage = "Yor password must be a mix of Alphanumeric and special characters")]
        public string Password { get; set; } = string.Empty;

        public bool Remember { get; set; } = false;
    }
}
