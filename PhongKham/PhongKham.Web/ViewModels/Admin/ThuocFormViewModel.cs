using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhongKham.Web.ViewModels.Admin;

public class ThuocFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nhóm thuốc")]
    [Required(ErrorMessage = "Vui lòng chọn nhóm thuốc.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhóm thuốc.")]
    public int MedicineCategoryId { get; set; }

    [Display(Name = "Tên thuốc")]
    [Required(ErrorMessage = "Vui lòng nhập tên thuốc.")]
    [StringLength(150, ErrorMessage = "Tên thuốc không được vượt quá 150 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Hoạt chất")]
    [StringLength(200, ErrorMessage = "Hoạt chất không được vượt quá 200 ký tự.")]
    public string? ActiveIngredient { get; set; }

    [Display(Name = "Đơn vị tính")]
    [Required(ErrorMessage = "Vui lòng nhập đơn vị tính.")]
    [StringLength(30, ErrorMessage = "Đơn vị tính không được vượt quá 30 ký tự.")]
    public string Unit { get; set; } = string.Empty;

    [Display(Name = "Đơn giá (đ)")]
    [Required(ErrorMessage = "Vui lòng nhập đơn giá.")]
    [Range(0, 1000000000, ErrorMessage = "Đơn giá không được nhỏ hơn 0.")]
    public decimal Price { get; set; }

    [Display(Name = "Số lượng tồn kho")]
    [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho.")]
    [Range(0, 10000000, ErrorMessage = "Số lượng tồn kho không được nhỏ hơn 0.")]
    public int StockQuantity { get; set; }

    [Display(Name = "Hãng sản xuất")]
    [StringLength(150, ErrorMessage = "Hãng sản xuất không được vượt quá 150 ký tự.")]
    public string? Manufacturer { get; set; }

    [Display(Name = "Đang sử dụng")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> NhomThuocOptions { get; set; } = new();
}
