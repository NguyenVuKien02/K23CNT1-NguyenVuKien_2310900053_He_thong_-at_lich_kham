namespace PhongKham.Business.DTOs.Doctors;

public class DanhGiaBacSiPublicDto
{
    public int Id { get; set; }

    public string TenBenhNhan { get; set; } = string.Empty;

    public int SoSao { get; set; }

    public string? NhanXet { get; set; }

    public DateTime NgayDanhGia { get; set; }
}
