using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Account
{
    public class UpdateUserInfoRequestDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id is required")]
        public string Id { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 1)]
        public string UserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        public EnumStatus? Status { get; set; }

        [Required(ErrorMessage = "Roles is required")]
        public List<CreateRoleRequestDTO> Roles { get; set; }
    }
}
