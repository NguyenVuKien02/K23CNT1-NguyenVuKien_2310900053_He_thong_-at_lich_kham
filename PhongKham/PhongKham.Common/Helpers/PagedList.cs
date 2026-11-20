namespace PhongKham.Common.Helpers;

public class PagedList<T>
{
    public const int DefaultPageSize = 10;

    public IReadOnlyList<T> Items { get; }

    public int PageIndex { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public int StartItem => TotalCount == 0 ? 0 : ((PageIndex - 1) * PageSize) + 1;

    public int EndItem => Math.Min(PageIndex * PageSize, TotalCount);

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize = DefaultPageSize)
    {
        Items = items;
        TotalCount = Math.Max(0, totalCount);
        PageSize = pageSize < 1 ? DefaultPageSize : pageSize;
        PageIndex = pageIndex < 1 ? 1 : pageIndex;
    }

    public static PagedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize = DefaultPageSize)
    {
        var normalizedPageIndex = pageIndex < 1 ? 1 : pageIndex;
        var normalizedPageSize = pageSize < 1 ? DefaultPageSize : pageSize;

        var list = source as IReadOnlyCollection<T> ?? source.ToList();
        var totalCount = list.Count;
        var items = list
            .Skip((normalizedPageIndex - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToList();

        return new PagedList<T>(items, totalCount, normalizedPageIndex, normalizedPageSize);
    }

    public static PagedList<T> Empty(int pageSize = DefaultPageSize) =>
        new([], 0, 1, pageSize);
}
