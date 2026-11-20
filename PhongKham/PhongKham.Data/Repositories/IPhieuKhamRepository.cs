using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public interface IPhieuKhamRepository : IRepository<PhieuKham>
{
    Task<PhieuKham?> GetByLichHenIdAsync(int lichHenId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PhieuKham>> GetLichSuKhamAsync(int hoSoBenhAnId, CancellationToken cancellationToken = default);

    Task<PhieuKham?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default);
}
