namespace PhongKham.Web.ViewModels.Admin;

public class AdminPaginationViewModel
{
    public int PageIndex { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int StartItem { get; set; }

    public int EndItem { get; set; }

    public string ItemLabel { get; set; } = "bản ghi";

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";

    public Dictionary<string, string?> RouteValues { get; set; } = new();

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;
}
