using System.ComponentModel.DataAnnotations;
using PhongKham.Common.Constants;
using PhongKham.Common.Enums;

namespace PhongKham.Web.ViewModels.Admin;

public class NhanVienCreateViewModel
{
    [Display(Name = "Email đăng nhập")]
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Mật khẩu khởi tạo")]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = QuyDinh.DoDaiMatKhauToiThieu, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Xác nhận mật khẩu")]
    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

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
    public string? Position { get; set; } = "Lễ tân";

    [Display(Name = "Ngày vào làm")]
    [DataType(DataType.Date)]
    public DateOnly? HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; } = true;
}
