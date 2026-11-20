using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public interface ILichHenRepository : IRepository<LichHen>
{
    Task<bool> ExistsAsync(int doctorId, DateOnly ngayHen, TimeOnly gioBatDau, CancellationToken cancellationToken = default);

    Task<bool> CoTrungGioAsync(int patientId, DateOnly ngayHen, TimeOnly gioBatDau, int? excludeLichHenId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LichHen>> GetByDoctorAndNgayAsync(int doctorId, DateOnly ngayHen, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LichHen>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default);

    Task<LichHen?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default);
}
