using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_API.Controllers.Admin
{
    public class Controller1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
