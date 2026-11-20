using System;
using System.Collections.Generic;
using PhongKham.Common.Enums;

namespace PhongKham.Data.Entities;

public partial class Staff
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public GioiTinh? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual User User { get; set; } = null!;
}
