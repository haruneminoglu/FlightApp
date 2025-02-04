using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightApp.Data;
using FlightApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightApp.Controllers
{
    public class AdminFlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminFlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Uçuşların listelenmesi (vw_flights kullanılıyor)
        public async Task<IActionResult> Index()
        {
            var flights = await _context.VwFlights.ToListAsync(); // vw_flights view'ı kullanılıyor
            return View(flights);
        }

        // Yeni uçuş ekleme (Create)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Airlines = new SelectList(_context.Airlines, "AirlineId", "AirlineName");
            return View();
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Flight flight)
{

    if (ModelState.IsValid)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_CreateFlight({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                flight.DepartureCity, 
                flight.ArrivalCity, 
                flight.DepartureTime.ToString("HH:mm:ss"), 
                flight.ArrivalTime.ToString("HH:mm:ss"), 
                flight.FlightDate, 
                flight.Price, 
                flight.AirlineId
            );

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Veritabanı hatası: " + ex.Message);
        }
    }

    ViewBag.Airlines = new SelectList(_context.Airlines, "AirlineId", "AirlineName");
    return View(flight);
}



        // Uçuş düzenleme (Edit)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            ViewBag.Airlines = new SelectList(_context.Airlines, "AirlineId", "AirlineName", flight.AirlineId);
            return View(flight);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Flight flight)
{
    if (ModelState.IsValid)
    {
        try
        {
            // Saat ve tarih formatları için doğru dönüşüm
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_UpdateFlight({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                flight.FlightId,
                flight.DepartureCity,
                flight.ArrivalCity,
                flight.DepartureTime.ToString("HH:mm:ss"),
                flight.ArrivalTime.ToString("HH:mm:ss"),
                flight.FlightDate.ToString("yyyy-MM-dd"),
                flight.Price,
                flight.AirlineId
            );

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Konsola hata loglama
            Console.WriteLine($"Hata: {ex.Message}\n{ex.StackTrace}");
            ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu: " + ex.Message);
        }
    }

    // Hata durumunda Airlines listesini tekrar yükleme
    ViewBag.Airlines = new SelectList(_context.Airlines, "AirlineId", "AirlineName", flight.AirlineId);

    // ModelState hatalarını loglama
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
    }

    return View(flight);
}

        // GET: Delete Confirmation
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Flight verisini getirme
    var flight = await _context.Flights
        .Include(f => f.Airline) // Havayolu bilgisi için Include
        .FirstOrDefaultAsync(m => m.FlightId == id);

    if (flight == null)
    {
        return NotFound();
    }

    // Flight bilgisi Delete view'e gönderilir
    return View(flight);
}

// POST: Delete Flight
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    try
    {
        // Stored Procedure çağrısı
        await _context.Database.ExecuteSqlRawAsync(
            "CALL sp_DeleteFlight({0})", id);

        // Başarılı silme sonrası Index sayfasına yönlendirme
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        // Hata durumunda flight bilgisi tekrar yüklenir
        var flight = await _context.Flights
            .Include(f => f.Airline)
            .FirstOrDefaultAsync(m => m.FlightId == id);

        if (flight == null)
        {
            return NotFound();
        }

        // Hata mesajını ModelState'e ekler
        ModelState.AddModelError("", "Silme işlemi sırasında bir hata oluştu: " + ex.Message);
        return View("Delete", flight); // Aynı Delete view'i tekrar yüklenir
    }
}


}}
