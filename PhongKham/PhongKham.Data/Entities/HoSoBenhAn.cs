using System;
using System.Collections.Generic;

namespace PhongKham.Data.Entities;

public partial class HoSoBenhAn
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public string MaHoSo { get; set; } = null!;

    public string? NhomMau { get; set; }

    public string? DiUng { get; set; }

    public string? TienSuBenh { get; set; }

    public string? BenhManTinh { get; set; }

    public string? TienSuGiaDinh { get; set; }

    public string? NguoiLienHe { get; set; }

    public string? SdtNguoiLienHe { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<PhieuKham> PhieuKhams { get; set; } = new List<PhieuKham>();
}
