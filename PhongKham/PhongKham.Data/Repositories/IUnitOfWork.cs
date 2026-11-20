namespace PhongKham.Data.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    ILichHenRepository LichHens { get; }

    IPhieuKhamRepository PhieuKhams { get; }

    IHoaDonRepository HoaDons { get; }

    IRepository<T> Repository<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
