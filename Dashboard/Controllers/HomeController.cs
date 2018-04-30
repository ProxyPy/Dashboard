using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "This is your credentials";

            return View();
        }

        [HttpPost]
        public ActionResult Index(string consumerKey,
            string consumerKeySecret,
            string accessToken,
            string accessTokenSecret)
        {
            ViewBag.Message = $"{consumerKey}:{consumerKeySecret} || {accessToken}:{accessTokenSecret}";
            return View();
        }

        public ActionResult TimeLine()
        {
            var key = 123;
            var userId = 42;

            var text = "text you got from calling the API";

            return View("Timeline", new TimelineViewModel { Text = text });
        }
    }
}