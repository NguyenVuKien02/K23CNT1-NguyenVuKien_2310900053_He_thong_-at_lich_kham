using System.ComponentModel.DataAnnotations;

namespace PhongKham.Web.ViewModels.Admin;

public class NhomThuocFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tên nhóm thuốc")]
    [Required(ErrorMessage = "Vui lòng nhập tên nhóm thuốc.")]
    [StringLength(100, ErrorMessage = "Tên nhóm thuốc không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [StringLength(300, ErrorMessage = "Mô tả không được vượt quá 300 ký tự.")]
    public string? Description { get; set; }
}
