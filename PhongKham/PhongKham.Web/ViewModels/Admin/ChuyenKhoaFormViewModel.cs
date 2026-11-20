using System.ComponentModel.DataAnnotations;

namespace PhongKham.Web.ViewModels.Admin;

public class ChuyenKhoaFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tên chuyên khoa")]
    [Required(ErrorMessage = "Vui lòng nhập tên chuyên khoa.")]
    [StringLength(100, ErrorMessage = "Tên chuyên khoa không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; } = true;
}
