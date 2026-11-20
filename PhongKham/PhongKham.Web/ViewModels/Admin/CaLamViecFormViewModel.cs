using System.ComponentModel.DataAnnotations;

namespace PhongKham.Web.ViewModels.Admin;

public class CaLamViecFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tên ca làm việc")]
    [Required(ErrorMessage = "Vui lòng nhập tên ca làm việc.")]
    [StringLength(50, ErrorMessage = "Tên ca làm việc không được vượt quá 50 ký tự.")]
    public string TenCa { get; set; } = string.Empty;

    [Display(Name = "Giờ bắt đầu")]
    [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu.")]
    public TimeOnly GioBatDau { get; set; } = new(7, 30);

    [Display(Name = "Giờ kết thúc")]
    [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc.")]
    public TimeOnly GioKetThuc { get; set; } = new(11, 30);

    [Display(Name = "Thời lượng mỗi khung giờ (phút)")]
    [Required(ErrorMessage = "Vui lòng nhập thời lượng khung giờ.")]
    [Range(5, 240, ErrorMessage = "Thời lượng khung giờ phải từ 5 đến 240 phút.")]
    public int ThoiLuongKhungGio { get; set; } = 30;
}
