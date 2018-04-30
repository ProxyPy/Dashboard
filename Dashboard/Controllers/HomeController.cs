using Dashboard.Models;
using Newtonsoft.Json;
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

        //[HttpPost]
        //public ActionResult Index(string consumerKey,
        //    string consumerKeySecret,
        //    string accessToken,
        //    string accessTokenSecret)
        //{
        //    ViewBag.Message = $"{consumerKey}:{consumerKeySecret} || {accessToken}:{accessTokenSecret}";
        //    return View();
        //}

        public ActionResult TimeLine()
        {
            string _consumerSecret = "cJOhqjzyM867Hm5jJLv9POBDqMqpsXZpU6igowRqrYCh5BqACN"; // Ici pour le POC
            string _accessTokenSecret = "joPEpXOvUvUCuQjTFRlwGflWc3B3tY5z8XtBepK9Mxn9h"; // Ici pour le POC


            var twitter = new API.Twitter.MyTwitterAPI(_consumerSecret, _accessTokenSecret);
            var text = twitter.GetTimeline();

            //return View("Timeline", new TimelineViewModel { Text = text });
            var model = new TimelineViewModel();
            model.Tweets = JsonConvert.DeserializeObject<List<Models.Tweet>>(text);
            return View("Timeline", model.Tweets);
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a proof of concept about the Dashboard";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My email is lucas.lavy-upsdale@epitech.eu";

            return View();
        }
    }
}