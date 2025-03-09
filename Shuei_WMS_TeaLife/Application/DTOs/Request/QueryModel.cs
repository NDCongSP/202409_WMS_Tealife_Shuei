namespace Application.DTOs.Request
{
    public class QueryModel<TEntity> where TEntity : class
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public TEntity Entity { get; set; }
        //public IDictionary<string, SortDesc>
    }

    public class SortDesc
    {
        public int Order { get; set; }
        public SortDirection Direction { get; set; }
    }
    public enum SortDirection
    {
        ASC,
        DESC
    }
}
