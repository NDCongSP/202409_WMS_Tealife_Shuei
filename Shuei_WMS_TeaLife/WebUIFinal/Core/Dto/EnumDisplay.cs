namespace WebUIFinal.Core.Dto
{
    public class EnumDisplay<T>
    {
        public required T Value { get; set; }
        public required string DisplayValue { get; set; }
    }
}
