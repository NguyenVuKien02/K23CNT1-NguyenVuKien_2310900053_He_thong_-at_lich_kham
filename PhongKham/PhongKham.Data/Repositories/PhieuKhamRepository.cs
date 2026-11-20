using Microsoft.EntityFrameworkCore;
using PhongKham.Data.Context;
using PhongKham.Data.Entities;

namespace PhongKham.Data.Repositories;

public class PhieuKhamRepository : Repository<PhieuKham>, IPhieuKhamRepository
{
    public PhieuKhamRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PhieuKham?> GetByLichHenIdAsync(int lichHenId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(pk => pk.HoSoBenhAn)
                .ThenInclude(hs => hs.Patient)
            .Include(pk => pk.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(pk => pk.LichHen)
            .Include(pk => pk.DonThuocs)
                .ThenInclude(dt => dt.ChiTietDonThuocs)
                    .ThenInclude(ct => ct.Medicine)
            .Include(pk => pk.ChiDinhDichVus)
                .ThenInclude(cd => cd.DichVu)
            .Include(pk => pk.HoaDon)
            .FirstOrDefaultAsync(pk => pk.LichHenId == lichHenId, cancellationToken);
    }

    public async Task<IReadOnlyList<PhieuKham>> GetLichSuKhamAsync(int hoSoBenhAnId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(pk => pk.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(pk => pk.DonThuocs)
                .ThenInclude(dt => dt.ChiTietDonThuocs)
                    .ThenInclude(ct => ct.Medicine)
            .Include(pk => pk.ChiDinhDichVus)
                .ThenInclude(cd => cd.DichVu)
            .Where(pk => pk.HoSoBenhAnId == hoSoBenhAnId)
            .OrderByDescending(pk => pk.NgayKham)
            .ToListAsync(cancellationToken);
    }

    public async Task<PhieuKham?> GetWithChiTietAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(pk => pk.HoSoBenhAn)
                .ThenInclude(hs => hs.Patient)
            .Include(pk => pk.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(pk => pk.LichHen)
            .Include(pk => pk.DonThuocs)
                .ThenInclude(dt => dt.ChiTietDonThuocs)
                    .ThenInclude(ct => ct.Medicine)
            .Include(pk => pk.ChiDinhDichVus)
                .ThenInclude(cd => cd.DichVu)
            .Include(pk => pk.HoaDon)
            .FirstOrDefaultAsync(pk => pk.Id == id, cancellationToken);
    }
}
