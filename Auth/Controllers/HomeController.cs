using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;

namespace Auth.Controllers
{
    public class HomeController : Controller
    {
        BenutzerdatenverwaltungEntities _db = new BenutzerdatenverwaltungEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [User]
        public ActionResult Userpage()
        {
            return View();
        }

        [Admin]
        public ActionResult Adminpage()
        {
            return View();
        }
    }
}