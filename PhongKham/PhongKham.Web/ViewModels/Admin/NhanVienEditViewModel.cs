using System.ComponentModel.DataAnnotations;
using PhongKham.Common.Enums;

namespace PhongKham.Web.ViewModels.Admin;

public class NhanVienEditViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Display(Name = "Email đăng nhập")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên nhân viên.")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Giới tính")]
    public GioiTinh? Gender { get; set; }

    [Display(Name = "Số điện thoại")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    public string? Phone { get; set; }

    [Display(Name = "Chức vụ")]
    [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự.")]
    public string? Position { get; set; }

    [Display(Name = "Ngày vào làm")]
    [DataType(DataType.Date)]
    public DateOnly? HireDate { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; }
}
