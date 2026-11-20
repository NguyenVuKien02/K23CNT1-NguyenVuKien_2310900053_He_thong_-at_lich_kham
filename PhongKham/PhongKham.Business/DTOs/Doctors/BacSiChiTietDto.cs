using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Business.DTOs.Doctors;

public class BacSiChiTietDto
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string SpecialtyName { get; set; } = string.Empty;

    public string? SpecialtyDescription { get; set; }

    public string FullName { get; set; } = string.Empty;

    public GioiTinh? Gender { get; set; }

    public string? Degree { get; set; }

    public int? ExperienceYears { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public decimal ConsultationFee { get; set; }

    public double DiemDanhGiaTrungBinh { get; set; }

    public int SoLuotDanhGia { get; set; }

    public IReadOnlyDictionary<int, int> PhanBoSoSao { get; set; } = new Dictionary<int, int>();

    public IReadOnlyList<LichLamViecBacSiPublicDto> LichLamViecSapToi { get; set; } = [];

    public PagedList<DanhGiaBacSiPublicDto> DanhGia { get; set; } = PagedList<DanhGiaBacSiPublicDto>.Empty();
}
