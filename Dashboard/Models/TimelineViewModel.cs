using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    //public class TimelineViewModel
    //{
    //    public string Text { get; set; }
    //}
    public class TimelineViewModel
    {
        public List<Tweet> Tweets { get; set; }
    }
    public class Tweet
    {
        [JsonProperty("user")]
        public MyUser User { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}