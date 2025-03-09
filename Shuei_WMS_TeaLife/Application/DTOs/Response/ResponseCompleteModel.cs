namespace Application.DTOs.Response;

public class ResponseCompleteModel
{
    public int Total { get; set; }
    public int Success { get; set; }
    public ResponseCompleteModel(int total, int success)
    {
        this.Total = total;
        this.Success = success;
    }
}
