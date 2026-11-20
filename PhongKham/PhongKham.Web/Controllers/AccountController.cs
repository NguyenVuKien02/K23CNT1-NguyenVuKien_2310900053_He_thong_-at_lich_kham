using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Business.DTOs.Auth;
using PhongKham.Business.Services;
using PhongKham.Common.Constants;
using PhongKham.Web.ViewModels.Account;

namespace PhongKham.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByCurrentUserRole();
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByCurrentUserRole();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new RegisterPatientDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            Phone = model.Phone,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Address = model.Address
        };

        var result = await _authService.DangKyBenhNhanAsync(dto, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đăng ký không thành công.");
            return View(model);
        }

        TempData["Success"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập để sử dụng hệ thống.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByCurrentUserRole();
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByCurrentUserRole();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.DangNhapAsync(
            new LoginDto
            {
                Email = model.Email,
                Password = model.Password
            },
            cancellationToken);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đăng nhập không thành công.");
            return View(model);
        }

        var authUser = result.Data;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, authUser.UserId.ToString()),
            new(ClaimTypes.Name, authUser.FullName),
            new(ClaimTypes.Email, authUser.Email),
            new(ClaimTypes.Role, authUser.RoleName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        TempData["Success"] = $"Chào mừng {authUser.FullName} đăng nhập thành công!";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectByRole(authUser.RoleName);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Info"] = "Bạn đã đăng xuất khỏi hệ thống.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdRaw, out var userId))
        {
            TempData["Error"] = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _authService.DoiMatKhauAsync(
            new ChangePasswordDto
            {
                UserId = userId,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            },
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đổi mật khẩu không thành công.");
            return View(model);
        }

        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction(nameof(ChangePassword));
    }

    private IActionResult RedirectByCurrentUserRole()
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        return RedirectByRole(role);
    }

    private IActionResult RedirectByRole(string roleName) => roleName switch
    {
        RoleNames.Patient => Redirect("/"),
        RoleNames.Receptionist => Redirect("/LeTan/LichHen"),
        RoleNames.Doctor => Redirect("/BacSi/DanhSachCho"),
        RoleNames.Admin => Redirect("/Admin/ThongKe"),
        _ => Redirect("/")
    };
}
