namespace PhongKham.Web.ViewModels.Admin;

public class NhomThuocAdminItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SoThuoc { get; set; }
}
