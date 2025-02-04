// Controllers/AdminPassengersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlightApp.Services;
using FlightApp.Models;
namespace FlightApp.Controllers
{

 public class AdminPassengersController : Controller
{
    private readonly PassengerGrpcClient _passengerClient;

    public AdminPassengersController(PassengerGrpcClient passengerClient)
    {
        _passengerClient = passengerClient;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToAction("Login", "Account");
            
        var passengers = await _passengerClient.GetPassengersAsync();
        return View(passengers);
    }
}
}