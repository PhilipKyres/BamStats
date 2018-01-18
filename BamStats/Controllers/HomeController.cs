using BamStats.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace BamStats.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
		[Authorize]
        public ActionResult Index()
        {
			if(User.Identity.Name.Equals("admin"))
				return RedirectToAction("Index", "BamFights", new { });
			else
				return RedirectToAction("GetInfo", "BamFights", new { });
        }

		// GET: Login
		public ActionResult Login()
		{
			return View();
		}

		// Post: Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginVM vm)
		{
			if (ModelState.IsValid)
			{
				if(vm.Password.Equals("parku"))
				{
					FormsAuthentication.RedirectFromLoginPage("user", false);
					return RedirectToAction("Index");
				}
				else if (vm.Password.Equals("guest"))
				{
					FormsAuthentication.RedirectFromLoginPage("guest", false);
					return RedirectToAction("Index");
				}
				else if (vm.Password.Equals("mrcat"))
				{
					FormsAuthentication.RedirectFromLoginPage("admin", false);
					return RedirectToAction("Index");
				}
				ModelState.AddModelError("", "Incorrect password");
			}
			return View(vm);
		}

		[Authorize]
		public ActionResult Logout(String returnUrl)
		{
			FormsAuthentication.SignOut();

			if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}
    }
}