namespace PhongKham.Business.DTOs.DanhMuc;

public class ThuocAdminDto
{
    public int Id { get; set; }

    public int MedicineCategoryId { get; set; }

    public string MedicineCategoryName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ActiveIngredient { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? Manufacturer { get; set; }

    public bool IsActive { get; set; }
}
