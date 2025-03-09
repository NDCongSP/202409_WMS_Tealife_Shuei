
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SupplierTenantDTO: Supplier
    {
        public string? TenantName { get; set; }
    }
}