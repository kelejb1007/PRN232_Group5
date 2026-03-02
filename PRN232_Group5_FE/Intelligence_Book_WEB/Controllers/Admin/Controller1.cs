using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class Controller1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
