using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhongKham.Common.Enums;

namespace PhongKham.Web.ViewModels.Admin;

public class BacSiEditViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Display(Name = "Email đăng nhập")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Chuyên khoa")]
    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    public int SpecialtyId { get; set; }

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên bác sĩ.")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Giới tính")]
    public GioiTinh? Gender { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Số điện thoại")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    public string? Phone { get; set; }

    [Display(Name = "Học vị / Chức danh")]
    [StringLength(100, ErrorMessage = "Học vị / chức danh không được vượt quá 100 ký tự.")]
    public string? Degree { get; set; }

    [Display(Name = "Số năm kinh nghiệm")]
    [Range(0, 80, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 80.")]
    public int? ExperienceYears { get; set; }

    [Display(Name = "Phí khám (đ)")]
    [Required(ErrorMessage = "Vui lòng nhập phí khám.")]
    [Range(0, 1000000000, ErrorMessage = "Phí khám không được nhỏ hơn 0.")]
    public decimal ConsultationFee { get; set; }

    [Display(Name = "Ảnh đại diện (URL)")]
    [StringLength(300, ErrorMessage = "Đường dẫn ảnh không được vượt quá 300 ký tự.")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "Giới thiệu")]
    [StringLength(1000, ErrorMessage = "Giới thiệu không được vượt quá 1000 ký tự.")]
    public string? Bio { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; }

    public List<SelectListItem> ChuyenKhoaOptions { get; set; } = new();
}
