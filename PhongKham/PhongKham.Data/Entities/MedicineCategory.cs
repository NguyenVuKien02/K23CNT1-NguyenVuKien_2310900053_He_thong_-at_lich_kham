using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class MedicineCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
