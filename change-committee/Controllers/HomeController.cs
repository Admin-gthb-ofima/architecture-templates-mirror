using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace change_committee.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return RedirectToAction("Login", "Account");
    }
}
