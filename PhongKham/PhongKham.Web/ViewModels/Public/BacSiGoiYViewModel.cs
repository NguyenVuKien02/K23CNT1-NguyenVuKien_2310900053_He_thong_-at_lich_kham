namespace PhongKham.Web.ViewModels.Public;

public class BacSiGoiYViewModel
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Degree { get; set; }

    public string DisplayName => string.IsNullOrWhiteSpace(Degree)
        ? FullName
        : $"{Degree} {FullName}";
}
