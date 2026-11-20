using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Admin;

public class DichVuAdminItemViewModel
{
    public int Id { get; set; }

    public string MaDichVu { get; set; } = string.Empty;

    public string TenDichVu { get; set; } = string.Empty;

    public LoaiDichVu LoaiDichVu { get; set; }

    public decimal DonGia { get; set; }

    public string? MoTa { get; set; }

    public bool ConHieuLuc { get; set; }

    public int SoLuotChiDinh { get; set; }

    public string TenLoaiDichVu => EnumDisplayHelper.TenLoaiDichVu(LoaiDichVu);
}
