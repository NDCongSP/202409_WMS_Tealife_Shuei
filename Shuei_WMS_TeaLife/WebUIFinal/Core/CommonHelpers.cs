namespace WebUIFinal.Core
{
    public static class CommonHelpers
    {
        public static T ParseEnum<T>(string value) 
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            else
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
        }

        public static string EnumConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static string ParseLotno(System.DateOnly? _date, string _documentNo)
        {
            string ret;
            if (_date == null)
            {
                ret = _documentNo;
            }
            else
            {
                ret = _date.Value.ToString("yyyyMMdd");
            }
            return ret;
        }
    }
}
