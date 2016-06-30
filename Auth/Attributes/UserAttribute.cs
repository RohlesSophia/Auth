using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using System.Web.Routing;


namespace Auth.Models
{
    public class UserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AccountController account = new AccountController();
            
            //Get Session
            var context = filterContext.HttpContext;
            var session = context.Session["username"];

            //Testen ob die Session erstellt wurde:
            if(session == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
            else if (!account.UsernameExist(session.ToString()))
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }
}