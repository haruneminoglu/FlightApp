using Microsoft.AspNetCore.Mvc;

namespace FlightApp.ViewComponents
{
    public class FlightSearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
