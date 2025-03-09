namespace Application.DTOs.Response;

public class CreatePickingModel
{
    public bool Item1 { get; set; }
    public int Item2 { get; set; }
    public int Item3 { get; set; }
    public string Item4 { get; set; } = string.Empty;

    public CreatePickingModel(bool item1, int item2, int item3, string item4 = "")
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
        Item4 = item4;
    }
}
