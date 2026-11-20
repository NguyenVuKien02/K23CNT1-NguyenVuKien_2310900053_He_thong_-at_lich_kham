using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhongKham.Common.Constants;

namespace PhongKham.Web.Areas.BacSi.Controllers;

[Area("BacSi")]
[Authorize(Roles = RoleNames.Doctor)]
public class DanhSachChoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
