using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Public;

public class DichVuItemViewModel
{
    public int Id { get; set; }

    public string MaDichVu { get; set; } = string.Empty;

    public string TenDichVu { get; set; } = string.Empty;

    public LoaiDichVu LoaiDichVu { get; set; }

    public decimal DonGia { get; set; }

    public string? MoTa { get; set; }

    public string TenLoaiDichVu => EnumDisplayHelper.TenLoaiDichVu(LoaiDichVu);
}
