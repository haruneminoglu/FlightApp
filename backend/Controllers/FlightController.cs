using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using FlightApp.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace FlightApp.Controllers
{
    public class FlightController : Controller
    {
        private readonly string connectionString = "Server=localhost;Database=flightdb;Uid=root;Pwd=12345;";

        public IActionResult Routes()
        {
            // Örnek: Kullanıcının seçtiği şehirler
            string originCity = "Ankara";
            string destinationCity = "Istanbul";

            ViewBag.OriginCity = originCity;
            ViewBag.DestinationCity = destinationCity;

            return View();
        }

        [HttpGet]
        public IActionResult SearchResults(string departureCity, string arrivalCity, string flightDate)
        {
            try
            {
                // Debug: Gelen parametreleri yazdır
                Console.WriteLine("\n=== ARAMA PARAMETRELERİ ===");
                Console.WriteLine($"Gelen DepartureCity: {departureCity}");
                Console.WriteLine($"Gelen ArrivalCity: {arrivalCity}");
                Console.WriteLine($"Gelen FlightDate: {flightDate}");

                // Gelen arama parametreleri kontrol edilir
                if (string.IsNullOrEmpty(departureCity) || string.IsNullOrEmpty(arrivalCity) || string.IsNullOrEmpty(flightDate))
                {
                    ViewBag.Message = "Lütfen tüm arama kriterlerini girin.";
                    return View(new List<FlightDetail>());
                }

                List<FlightDetail> flights = new List<FlightDetail>();

                // Tarihi doğru formatta almak için dönüştürme işlemi
                DateTime parsedFlightDate;
                if (!DateTime.TryParseExact(flightDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedFlightDate))
                {
                    ViewBag.Message = "Geçerli bir tarih formatı girin (YYYY-MM-DD).";
                    return View(new List<FlightDetail>());
                }

                // Debug: Tarih parse edildikten sonra kontrol
                Console.WriteLine($"Parse edilmiş tarih: {parsedFlightDate:yyyy-MM-dd}");

                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                       string query = @"
SELECT 
    f.FlightId,
    a.AirlineName,
    CAST(f.DepartureTime AS CHAR) as DepartureTime,
    CAST(f.ArrivalTime AS CHAR) as ArrivalTime,
    CAST(CalculateFlightDuration(f.DepartureTime, f.ArrivalTime) AS CHAR) as Duration,
    f.Price
FROM flights f
JOIN airlines a ON f.AirlineId = a.AirlineId
WHERE LOWER(f.DepartureCity) = LOWER(@DepartureCity)
  AND LOWER(f.ArrivalCity) = LOWER(@ArrivalCity)
  AND DATE(f.FlightDate) = DATE(@FlightDate)";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            // Parametreleri temizleyerek ekle
                            command.Parameters.AddWithValue("@DepartureCity", departureCity.Trim());
                            command.Parameters.AddWithValue("@ArrivalCity", arrivalCity.Trim());
                            command.Parameters.AddWithValue("@FlightDate", parsedFlightDate.Date);

                            // Debug: SQL sorgusunu ve parametreleri yazdır
                            Console.WriteLine("\n=== SQL SORGUSU VE PARAMETRELER ===");
                            Console.WriteLine("SQL Sorgusu:");
                            Console.WriteLine(query);
                            Console.WriteLine("Parametre Değerleri:");
                            foreach (MySqlParameter param in command.Parameters)
                            {
                                Console.WriteLine($"{param.ParameterName}: {param.Value}");
                            }

                            connection.Open();

                            using (var reader = command.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("Hiçbir sonuç bulunamadı.");
                                }
                                else
                                {
                                    while (reader.Read())
                                    {
                                        // Süreyi TimeSpan'e dönüştür ve formatla
                                        TimeSpan duration = TimeSpan.Parse(reader.GetString("Duration"));
                                        string formattedDuration = FormatDuration(duration);

                                        flights.Add(new FlightDetail
{
    FlightId = reader.GetInt32("FlightId"),  // Bunu ekleyin
    AirlineName = reader.GetString("AirlineName"),
    DepartureTime = DateTime.Parse(reader.GetString("DepartureTime")),
    ArrivalTime = DateTime.Parse(reader.GetString("ArrivalTime")),
    Duration = formattedDuration,
    Price = reader.GetDecimal("Price")
});

                                        // Debug: Okunan veri
                                        Console.WriteLine($"Okunan Veri: {reader.GetString("AirlineName")}, {reader.GetString("DepartureTime")}, {reader.GetString("ArrivalTime")}, {formattedDuration}, {reader.GetDecimal("Price")}");
                                    }

                                    // Debug: Bulunan sonuç sayısı
                                    Console.WriteLine($"\nBulunan uçuş sayısı: {flights.Count}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n=== SORGU HATASI ===");
                        Console.WriteLine($"Hata mesajı: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        throw;
                    }
                }

                return View(flights);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== GENEL HATA ===");
                Console.WriteLine($"Hata mesajı: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ViewBag.Message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View(new List<FlightDetail>());
            }
        }

        // Süreyi formatlayan fonksiyon
        public static string FormatDuration(TimeSpan duration)
        {
            return $"{duration.Hours} saat {duration.Minutes} dk";
   
     }

 
        [HttpGet]
public IActionResult ConfirmSelection(int flightId)
{
 var userId = HttpContext.Session.GetInt32("UserId");
    if (!userId.HasValue)
    {
        return RedirectToAction("Login", "Account");
    }
    

    if (flightId <= 0)
    {
        ViewBag.ErrorMessage = "Geçersiz uçuş seçimi.";
        return View("Error");
    }

    FlightDetail? flightDetail = null;

    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            string query = @"
                SELECT 
                    f.FlightId,
                    f.DepartureCity,
                    f.ArrivalCity,
                    CAST(f.DepartureTime AS CHAR) as DepartureTime,
                    CAST(f.ArrivalTime AS CHAR) as ArrivalTime,
                    CAST(f.FlightDate AS CHAR) as FlightDate,
                    f.Price,
                    a.AirlineName,
                    CAST(CalculateFlightDuration(f.DepartureTime, f.ArrivalTime) AS CHAR) as Duration
                FROM flights f
                JOIN airlines a ON f.AirlineId = a.AirlineId
                WHERE f.FlightId = @FlightId";

            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FlightId", flightId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        TimeSpan duration = TimeSpan.Parse(reader.GetString("Duration"));
                        string formattedDuration = FormatDuration(duration);

                        flightDetail = new FlightDetail
                        {
                            FlightId = reader.GetInt32("FlightId"),
                            AirlineName = reader.GetString("AirlineName"),
                            DepartureCity = reader.GetString("DepartureCity"),
                            ArrivalCity = reader.GetString("ArrivalCity"),
                            DepartureTime = DateTime.Parse(reader.GetString("DepartureTime")),
                            ArrivalTime = DateTime.Parse(reader.GetString("ArrivalTime")),
                            FlightDate = DateTime.Parse(reader.GetString("FlightDate")),
                            Duration = formattedDuration,
                            Price = reader.GetDecimal("Price")
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            ViewBag.ErrorMessage = "Uçuş bilgileri alınırken bir hata oluştu.";
            return View("Error");
        }
    }

    if (flightDetail == null)
    {
        ViewBag.ErrorMessage = "Uçuş bulunamadı.";
        return View("Error");
    }

    return View(flightDetail);
}
[HttpPost]
public IActionResult SetSelectedFlightId([FromBody] dynamic data)
{
    int flightId = data.flightId;
    Console.WriteLine($"Debug: Ajax ile gelen Flight ID = {flightId}");
    
    if (flightId == 0)
    {
        return Json(new { success = false, message = "Geçersiz uçuş seçimi." });
    }

    return Json(new { success = true, message = "Uçuş seçimi başarılı.", flightId });
}


[HttpPost]
public IActionResult CheckAuthStatus()
{
    // Session üzerinden kullanıcı kontrolü
    var userId = HttpContext.Session.GetInt32("UserId");
    var isAuthenticated = userId.HasValue;
    
    Console.WriteLine($"CheckAuthStatus - UserId: {userId}, IsAuthenticated: {isAuthenticated}"); // Debug için
    
    if (!isAuthenticated)
    {
        return Json(new { isAuthenticated = false });
    }
    
    return Json(new { isAuthenticated = true });
}




[HttpGet]
public IActionResult CalculatePrice(int flightId, int classId)
{
    try
    {
        Console.WriteLine($"CalculatePrice çağrıldı - FlightId: {flightId}, ClassId: {classId}");
        decimal price = 0;
        
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand("SELECT CalculatePrice(@flightId, @classId)", connection))
            {
                command.Parameters.AddWithValue("@flightId", flightId);
                command.Parameters.AddWithValue("@classId", classId);
                var result = command.ExecuteScalar();
                
                if (result != null && result != DBNull.Value)
                {
                    price = Convert.ToDecimal(result);
                }
            }
        }
        
        Console.WriteLine($"Hesaplanan fiyat: {price}");
        return Json(new { success = true, price });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata oluştu: {ex.Message}");
        return Json(new { success = false, message = ex.Message });
    }
}

[HttpPost]
public IActionResult CompleteReservation([FromForm] Passenger passenger, int flightId, int classId)
{
    System.Diagnostics.Debug.WriteLine($"Gelen ClassId: {classId}");
    try
    {
     Console.WriteLine($"Received data - FlightId: {flightId}, ClassId: {classId}");
        Console.WriteLine($"Passenger info - Name: {passenger.PassengerName}, Surname: {passenger.PassengerSurname}");

        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return Json(new { success = false, message = "Oturum bulunamadı. Lütfen tekrar giriş yapın." });
        }
        
        // Veritabanı işlemleri...
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Yolcu bilgilerini kaydet
                    var passengerCommand = new MySqlCommand(@"
                        INSERT INTO passengers (Gender, TcNo, PassengerName, PassengerSurname, DateOfBirth)
                        VALUES (@Gender, @TcNo, @PassengerName, @PassengerSurname, @DateOfBirth);
                        SELECT LAST_INSERT_ID();", connection, transaction);

                    passengerCommand.Parameters.AddWithValue("@Gender", passenger.Gender);
                    passengerCommand.Parameters.AddWithValue("@TcNo", passenger.TcNo);
                    passengerCommand.Parameters.AddWithValue("@PassengerName", passenger.PassengerName);
                    passengerCommand.Parameters.AddWithValue("@PassengerSurname", passenger.PassengerSurname);
                    passengerCommand.Parameters.AddWithValue("@DateOfBirth", passenger.DateOfBirth.ToString("yyyy-MM-dd"));

                    int passengerId = Convert.ToInt32(passengerCommand.ExecuteScalar());

                    // Rezervasyon kaydını oluştur
                    var reservationCommand = new MySqlCommand(@"
                        INSERT INTO reservations (UserId, FlightId, PassengerId, ClassId, ReservationDate)
                        VALUES (@UserId, @FlightId, @PassengerId, @ClassId, NOW());", connection, transaction);

                    reservationCommand.Parameters.AddWithValue("@UserId", userId);
                    reservationCommand.Parameters.AddWithValue("@FlightId", flightId);
                    reservationCommand.Parameters.AddWithValue("@PassengerId", passengerId);
                    reservationCommand.Parameters.AddWithValue("@ClassId", classId);

                    reservationCommand.ExecuteNonQuery();

                    transaction.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                   transaction.Rollback();
                    Console.WriteLine($"Database error: {ex.Message}");
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
    catch (Exception ex)
    {
         Console.WriteLine($"General error: {ex.Message}");
        return Json(new { success = false, message = ex.Message });
    }
}


    }
}
