using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhongKham.Business.Seeding;
using PhongKham.Business.Services;
using PhongKham.Data.Context;
using PhongKham.Data.Entities;
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

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<ILoginLockoutService, LoginLockoutService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDanhMucService, DanhMucService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<ITaiKhoanService, TaiKhoanService>();
        services.AddScoped<DbSeeder>();

        return services;
    }

    public static async Task SeedDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
