// Controllers/AdminUsersController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace FlightApp.Controllers
{
   public class AdminUsersController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AdminUsersController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

   public async Task<IActionResult> GetUsers()
{
    try
    {
        var response = await _httpClient.GetAsync("http://localhost:3000/api/users");
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"API yanıtı: {responseContent}"); // Gelen veriyi görelim
        
        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<AdminUserView>>();
            if (users != null && users.Any())
            {
                Console.WriteLine($"İlk kullanıcı örneği: {System.Text.Json.JsonSerializer.Serialize(users.First())}");
            }
            return Json(users);
        }
        
        return Json(new { error = "Kullanıcılar getirilemedi" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata oluştu: {ex.Message}");
        return Json(new { error = ex.Message });
    }
}
}
}