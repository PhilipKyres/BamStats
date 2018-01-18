using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantReview.Validators
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AdminAuthorize : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext.User.Identity.Name.Equals("admin"))
				return true;

			return false;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			//check if the user is authenticated: if s/he is, it means that the request is 
			// unauthorized because of their User name
			if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
			{
				//set a name value pair in TempData
				try
				{
					filterContext.Controller.TempData.Add("RedirectReason", "Unauthorized Request - Login with the permissions to access the page");
				}
				catch(ArgumentException) {}
			}
			base.HandleUnauthorizedRequest(filterContext);
		}

		public static bool isAdmin()
		{
			if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.Name.Equals("admin"))
			{
				return true;
			}
			return false;
		}
	}

}