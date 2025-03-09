using Mapster;
namespace Application.Extentions.Pagings;
public class PageList<TDto>
{
    public List<TDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;
    public PageList()
    {
        TotalItems = 0;
        PageSize = 0;
        CurrentPage = 0;
        TotalPages = 0;
        Items = new List<TDto>();
    }

    public PageList(List<TDto> items, int totalItems, int currentPage, int pageSize)
    {
        TotalItems = totalItems;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        Items = items;
    }

    public static PageList<TDto> PagedResult<TEntity>(IQueryable<TEntity> source, int currentPage, int pageSize)
    {
        var totalItems = source.Count();
        var items = source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        if (typeof(TDto) == typeof(TEntity))
        {
            var sameTypeItems = items.Cast<TDto>().ToList();
            return new PageList<TDto>(sameTypeItems, totalItems, currentPage, pageSize);
        }
        else
        {
            var dtoItems = items.Adapt<List<TDto>>();
            return new PageList<TDto>(dtoItems, totalItems, currentPage, pageSize);
        }
    }
}
