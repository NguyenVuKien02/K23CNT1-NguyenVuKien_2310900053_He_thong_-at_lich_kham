using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Common.Constants;

namespace PhongKham.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ThongKeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
