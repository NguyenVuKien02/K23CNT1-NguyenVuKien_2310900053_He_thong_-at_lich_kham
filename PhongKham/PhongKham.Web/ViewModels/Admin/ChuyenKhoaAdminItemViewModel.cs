namespace PhongKham.Web.ViewModels.Admin;

public class ChuyenKhoaAdminItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int SoBacSi { get; set; }
}
