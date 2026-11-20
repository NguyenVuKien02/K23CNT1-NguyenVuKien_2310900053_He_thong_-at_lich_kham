using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();

    public virtual ICollection<LichSuLichHen> LichSuLichHens { get; set; } = new List<LichSuLichHen>();

    public virtual Patient? Patient { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
