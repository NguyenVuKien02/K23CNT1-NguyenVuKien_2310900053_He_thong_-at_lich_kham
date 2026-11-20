using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhongKham.Common.Constants;
using PhongKham.Data.Entities;
using PhongKham.Data.Repositories;

namespace PhongKham.Business.Seeding;

public class DbSeeder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<DbSeeder> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var hasUsers = await userRepo.Query().AnyAsync(cancellationToken);
        if (hasUsers)
        {
            return;
        }

        var adminEmail = _configuration["Seed:AdminEmail"]?.Trim().ToLowerInvariant();
        var adminPassword = _configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            _logger.LogWarning(
                "Chưa cấu hình Seed:AdminEmail hoặc Seed:AdminPassword trong user-secrets. Bỏ qua bước tạo tài khoản Admin mặc định.");
            return;
        }

        var adminRole = await _unitOfWork.Repository<Role>().Query()
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Admin, cancellationToken);

        if (adminRole is null)
        {
            _logger.LogError("Không tìm thấy vai trò '{RoleName}' trong bảng Roles. Không thể tạo tài khoản Admin.", RoleNames.Admin);
            return;
        }

        var adminUser = new User
        {
            RoleId = adminRole.Id,
            Email = adminEmail,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, adminPassword);

        await userRepo.AddAsync(adminUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Đã khởi tạo tài khoản Admin mặc định: {Email}", adminEmail);
    }
}
