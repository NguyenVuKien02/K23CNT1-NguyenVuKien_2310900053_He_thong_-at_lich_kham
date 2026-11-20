using PhongKham.Common.Enums;
using PhongKham.Common.Helpers;

namespace PhongKham.Web.ViewModels.Public;

public class BacSiDetailsViewModel
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

    public IReadOnlyList<LichLamViecBacSiViewModel> LichLamViecSapToi { get; set; } = [];

    public PagedList<DanhGiaBacSiViewModel> DanhGia { get; set; } = PagedList<DanhGiaBacSiViewModel>.Empty();

    public string DisplayName => string.IsNullOrWhiteSpace(Degree)
        ? FullName
        : $"{Degree} {FullName}";

    public string Initials
    {
        get
        {
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "BS";
            }

            if (parts.Length == 1)
            {
                return parts[0][..1].ToUpperInvariant();
            }

            return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
        }
    }
}
