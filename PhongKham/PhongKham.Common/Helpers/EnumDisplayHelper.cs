using PhongKham.Common.Constants;
using PhongKham.Common.Enums;

namespace PhongKham.Common.Helpers;

public static class EnumDisplayHelper
{
    public static string TenLoaiDichVu(LoaiDichVu loai) => loai switch
    {
        LoaiDichVu.Kham => "Khám bệnh",
        LoaiDichVu.XetNghiem => "Xét nghiệm",
        LoaiDichVu.ChanDoanHinhAnh => "Chẩn đoán hình ảnh",
        LoaiDichVu.ThuThuat => "Thủ thuật",
        _ => "Khác"
    };

    public static string TenGioiTinh(GioiTinh? gioiTinh) => gioiTinh switch
    {
        GioiTinh.Nam => "Nam",
        GioiTinh.Nu => "Nữ",
        GioiTinh.Khac => "Khác",
        _ => "Chưa cập nhật"
    };

    public static string TenThuTrongTuan(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => "Thứ 2",
        DayOfWeek.Tuesday => "Thứ 3",
        DayOfWeek.Wednesday => "Thứ 4",
        DayOfWeek.Thursday => "Thứ 5",
        DayOfWeek.Friday => "Thứ 6",
        DayOfWeek.Saturday => "Thứ 7",
        DayOfWeek.Sunday => "Chủ nhật",
        _ => string.Empty
    };

    public static string TenVaiTro(string? roleName) => roleName switch
    {
        RoleNames.Admin => "Quản trị viên",
        RoleNames.Receptionist => "Lễ tân",
        RoleNames.Doctor => "Bác sĩ",
        RoleNames.Patient => "Bệnh nhân",
        _ => roleName ?? "Chưa xác định"
    };
}
