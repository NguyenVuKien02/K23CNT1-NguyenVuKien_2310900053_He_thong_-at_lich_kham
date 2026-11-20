using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class Medicine
{
    public int Id { get; set; }

    public int MedicineCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? ActiveIngredient { get; set; }

    public string Unit { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? Manufacturer { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();

    public virtual MedicineCategory MedicineCategory { get; set; } = null!;
}
