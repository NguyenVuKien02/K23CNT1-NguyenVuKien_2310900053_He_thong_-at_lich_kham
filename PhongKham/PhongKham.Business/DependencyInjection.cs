using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhongKham.Data.Context;
using PhongKham.Data.Repositories;

namespace PhongKham.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ILichHenRepository, LichHenRepository>();
        services.AddScoped<IPhieuKhamRepository, PhieuKhamRepository>();
        services.AddScoped<IHoaDonRepository, HoaDonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
