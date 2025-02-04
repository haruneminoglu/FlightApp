using Microsoft.AspNetCore.Mvc;
using FlightApp.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FlightApp.Models;




namespace FlightApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

  [HttpGet] // GET methodu için login sayfasını göster
public IActionResult Login(string? returnUrl = null)
{
    ViewBag.ReturnUrl = returnUrl;
    return View();
}

[HttpPost]
public IActionResult Login(string email, string password, string? returnUrl = null)
{
    if (_context.Users == null)
    {
        ViewBag.Error = "Veritabanı bağlantısı yapılamadı.";
        return View();
    }

    var user = _context.Users.FirstOrDefault(u => u.Email == email);

    if (user == null || user.Password != password)
    {
        ViewBag.Error = "E-posta veya şifre yanlış.";
        return View();
    }

    // Session'ı ayarla
    HttpContext.Session.SetInt32("UserId", user.UserId);
    HttpContext.Session.SetString("UserRole", user.Role);
    HttpContext.Session.SetString("UserName", $"{user.Firstname} {user.Lastname}");
    
    Console.WriteLine($"Login successful - UserId: {user.UserId}"); // Debug log

    // ReturnUrl kontrolü
    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
    {
        return Redirect(returnUrl);
    }

    return RedirectToAction("Index", "Home");
}




 [HttpPost]
public IActionResult Register(string firstname, string lastname, string email, string password, string confirmPassword)
{
    // Şifre ve şifre tekrar uyuşmuyorsa
    if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || password != confirmPassword)
    {
        ViewBag.Error = "Şifreler uyuşmuyor.";
        return View("Register"); // Kullanıcıyı kayıt ekranına yönlendir
    }

    // Veritabanı kontrolü
    if (_context.Users == null)
    {
        ViewBag.Error = "Veritabanı bağlantısı yapılamadı.";
        return View("Register");
    }

    // E-posta zaten kayıtlıysa
    if (_context.Users.Any(u => u.Email == email))
    {
        ViewBag.Error = "Bu e-posta zaten kayıtlı.";
        return View("Register");
    }

    // Yeni kullanıcı oluşturuluyor
    var user = new User
    {
        Firstname = firstname,
        Lastname = lastname,
        Email = email,
        Password = password, // Şifreyi düz metin olarak saklamayın! Hashleme eklemelisiniz.
        Role = "User"
    };

    _context.Users.Add(user);
    _context.SaveChanges();

    // Başarılı kayıt mesajı gösteriliyor
    ViewBag.Message = "Üyelik işleminiz başarılı! Şimdi giriş yapabilirsiniz.";
    return View("Register"); // Kullanıcıyı yine kayıt ekranında tutabilirsiniz veya giriş sayfasına yönlendirebilirsiniz.
}

[HttpPost]
public IActionResult Logout()
{
    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    HttpContext.Session.Clear(); // Oturumu tamamen temizliyoruz.
    return RedirectToAction("Index", "Home");
}




[HttpGet]
public IActionResult Profile()
{
    // Kullanıcı ID'sini oturumdan al
    int? userId = HttpContext.Session.GetInt32("UserId");
    if (!userId.HasValue)
    {
        return RedirectToAction("Login", "Account"); // Giriş sayfasına yönlendir
    }

    // Kullanıcının profil bilgilerini al
    var userProfile = _context.UserProfiles
        .SingleOrDefault(u => u.UserId == userId.Value); // Tekil profil sorgusu

    if (userProfile == null)
    {
        return NotFound("Kullanıcı profili bulunamadı.");
    }

    return View(userProfile);
}


[HttpGet]
public IActionResult GetEditProfilePartial(int userId)
{
    if (_context.UserProfiles == null)
    {
        return StatusCode(500, "Veritabanı bağlantısı kurulamadı.");
    }

    var userProfile = _context.UserProfiles.FirstOrDefault(up => up.UserId == userId);
    if (userProfile == null)
    {
        return NotFound("Kullanıcı profili bulunamadı.");
    }

    return PartialView("_EditProfilePartial", userProfile);
}








[HttpPost]
public IActionResult UpdateProfile(int userId, string fieldName, string newValue)
{
    if (_context.Users == null)
    {
        return StatusCode(500, "Veritabanı bağlantısı kurulamadı.");
    }

    if (string.IsNullOrEmpty(newValue))
    {
        return BadRequest("Yeni değer boş olamaz.");
    }

    // Kullanıcıyı bul
    var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
    if (user == null)
    {
        return NotFound("Kullanıcı bulunamadı.");
    }

    // Alanı güncelle
    switch (fieldName)
    {
        case "Firstname":
            user.Firstname = newValue;
            break;
        case "Lastname":
            user.Lastname = newValue;
            break;
        case "Email":
            user.Email = newValue;
            break;
        case "Password":
            // Şifreyi hashlemeyi unutmayın
            user.Password = newValue; // Not: Şifreyi hashlemeden saklamayın!
            break;
        default:
            return BadRequest("Geçersiz alan adı.");
    }

    // Değişiklikleri kaydet
    _context.SaveChanges();

    return Json(new { success = true });
}





    }
}
