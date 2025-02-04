using Microsoft.AspNetCore.Mvc;

namespace FlightApp.ViewComponents
{
    public class RouteMapViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string originCity, string destinationCity)
        {
            // Şehir bilgilerini view'e gönderiyoruz
            ViewBag.OriginCity = originCity;
            ViewBag.DestinationCity = destinationCity;

            return View();
        }
    }
}
