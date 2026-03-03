using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers.User
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
