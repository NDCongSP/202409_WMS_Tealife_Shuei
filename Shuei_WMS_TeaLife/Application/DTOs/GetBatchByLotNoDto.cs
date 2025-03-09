namespace Application.DTOs;

public class GetBatchByLotNoDto
{
    public int TenantId { get;set; }
    public string ProductCode { get; set; } = default!;
    public string LotNo { get; set; } = default!;
}
