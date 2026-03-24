using Intelligence_Book_WEB.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public abstract class BaseAdminController : Controller
    {
        protected readonly IProfileService _profileService;
        private const string AccessTokenCookie = "access_token";

        protected BaseAdminController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = Request.Cookies[AccessTokenCookie];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }

            try
            {
                var profile = await _profileService.GetProfileAsync(token);
                if (profile == null || (profile.Role != "Admin" && profile.Role != "1"))
                {
                    context.Result = RedirectToAction("Index", "Home");
                    return;
                }
                ViewBag.ProfileInfo = profile;
            }
            catch
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
