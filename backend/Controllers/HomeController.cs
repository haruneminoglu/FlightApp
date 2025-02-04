using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FlightApp.Controllers
{
    public class HomeController : Controller
    {
        // Ana sayfa (Index) ve rol kontrolü
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "AdminFlights"); // Adminler için uçuş işlemleri sayfasına yönlendir
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("UserPage"); // Normal kullanıcılar için UserPage yönlendirilebilir
            }

            return View(); // Eğer kullanıcı giriş yapmamışsa veya başka bir durumda ise normal sayfa göster
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        // Admin sayfası
        [Authorize(Roles = "Admin")] // Admin rolüne sahip olanların erişebileceği bir sayfa
        public IActionResult AdminPage()
        {
            return View();
        }

        // Kullanıcı sayfası
        [Authorize(Roles = "User")] // User rolüne sahip olanların erişebileceği bir sayfa
        public IActionResult UserPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EnableReturnDate()
        {
            // Bu endpoint sadece dönüş tarihini aktif hale getirecek
            return Json(new { success = true });
        }
    }
}