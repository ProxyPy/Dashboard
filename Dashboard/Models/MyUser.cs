using Newtonsoft.Json;

namespace Dashboard.Models
{
    public class MyUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
    }
}