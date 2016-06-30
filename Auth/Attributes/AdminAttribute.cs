using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using System.Web.Routing;


namespace Auth.Models
{
    public class AdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AccountController account = new AccountController();
            
            //Get Session
            var context = filterContext.HttpContext;
            var usernameFromSession = context.Session["username"];

            //Testen ob die Session erstellt wurde:
            if(usernameFromSession != null)
            {
                if (String.IsNullOrWhiteSpace(account.GetRole(usernameFromSession.ToString())))
                {
                    filterContext.Result = new RedirectResult("~/Home/Index");
                }
            }else
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }
}