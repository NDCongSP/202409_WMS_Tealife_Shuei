using System.ComponentModel.DataAnnotations;

namespace WebUIFinal.Core.Dto
{
    public class CompanyDisplayDto
    {
        [Required]
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public CompanyDisplayDto() { }
        public CompanyDisplayDto(CompanyTenant t)
        {
            Id = t.AuthPTenantId;
            CompanyName = t.FullName;
        }
    }
}
