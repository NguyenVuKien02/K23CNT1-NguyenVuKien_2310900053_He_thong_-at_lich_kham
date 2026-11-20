using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Common.Constants;

namespace PhongKham.Web.Areas.LeTan.Controllers;

[Area("LeTan")]
[Authorize(Roles = RoleNames.Receptionist)]
public class LichHenController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
