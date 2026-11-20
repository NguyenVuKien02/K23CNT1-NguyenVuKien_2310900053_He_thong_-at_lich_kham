namespace PhongKham.Business.DTOs.Doctors;

public class BacSiPublicDto
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string SpecialtyName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Degree { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public decimal ConsultationFee { get; set; }

    public double DiemDanhGiaTrungBinh { get; set; }

    public int SoLuotDanhGia { get; set; }
}
