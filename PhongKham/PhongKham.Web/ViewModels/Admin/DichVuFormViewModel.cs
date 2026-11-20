using System.ComponentModel.DataAnnotations;
using PhongKham.Common.Enums;

namespace PhongKham.Web.ViewModels.Admin;

public class DichVuFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Mã dịch vụ")]
    [StringLength(20, ErrorMessage = "Mã dịch vụ không được vượt quá 20 ký tự.")]
    public string? MaDichVu { get; set; }

    [Display(Name = "Tên dịch vụ")]
    [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
    [StringLength(200, ErrorMessage = "Tên dịch vụ không được vượt quá 200 ký tự.")]
    public string TenDichVu { get; set; } = string.Empty;

    [Display(Name = "Loại dịch vụ")]
    [Required(ErrorMessage = "Vui lòng chọn loại dịch vụ.")]
    public LoaiDichVu LoaiDichVu { get; set; } = LoaiDichVu.Kham;

    [Display(Name = "Đơn giá (đ)")]
    [Required(ErrorMessage = "Vui lòng nhập đơn giá.")]
    [Range(0, 1000000000, ErrorMessage = "Đơn giá không được nhỏ hơn 0.")]
    public decimal DonGia { get; set; }

    [Display(Name = "Mô tả")]
    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; set; }

    [Display(Name = "Còn hiệu lực")]
    public bool ConHieuLuc { get; set; } = true;
}
