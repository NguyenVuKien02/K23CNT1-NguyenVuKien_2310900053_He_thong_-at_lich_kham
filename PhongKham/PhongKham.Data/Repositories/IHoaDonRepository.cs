using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public interface IHoaDonRepository : IRepository<HoaDon>
{
    Task<IReadOnlyList<HoaDon>> GetChuaThanhToanAsync(CancellationToken cancellationToken = default);

    Task<HoaDon?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default);

    Task<HoaDon?> GetByPhieuKhamIdAsync(int phieuKhamId, CancellationToken cancellationToken = default);
}
