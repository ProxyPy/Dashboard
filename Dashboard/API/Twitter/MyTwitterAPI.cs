using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Dashboard.API.Twitter
{
    public class MyTwitterAPI
    {
        public class Tweet
        {
            [JsonProperty("from_user")]
            public string UserName { get; set; }
            [JsonProperty("text")]
            public string Text { get; set; }

        }

        string _baseUrl = "https://api.twitter.com/1.1/";
        string _consumerKey = "k3yROJtZxNoF64VwaoJ4q0hm1"; // The application's consumer key
        string _consumerSecret = "cJOhqjzyM867Hm5jJLv9POBDqMqpsXZpU6igowRqrYCh5BqACN"; // The application's consumer secret
        string _accessToken = "985079514886754305-XH9Ul0jQHxfikyzg4rTWUW7u7N6PrC8"; // The access token granted after OAuth authorization
        string _accessTokenSecret = "dhcHI5xrNwvRUS2LKn7VIG8wVnALnkLWN2OJiOQU6UmZZ"; // The access token secret granted after OAuth authorization
        readonly HMACSHA1 _sigHasher;
        DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        HttpClient _client;

        public MyTwitterAPI()
        {
            _sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes($"{_consumerSecret}&{_accessTokenSecret}"));
        }

        public List<Tweet> GetTimeline()
        {
            var buffer = MyHttpHelper.GetResposeFromUrl("https://api.twitter.com/1.1/statuses/home_timeline.json", _client);

            var json = JsonConvert.DeserializeObject<List<Tweet>>(buffer);
            return json;
        }

        public Task<string> SendTweet(string text)
        {
            var data = new Dictionary<string, string>
            {
                { "status", text },
                { "trim_user", "1" }
            };

            return MakeRequest("statuses/update.json", data);
        }

        private Task<string> MakeRequest(string url, Dictionary<string, string> data)
        {
            var fullUrl = _baseUrl + url;

            var timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);

            data.Add("oauth_consumer_key", _consumerKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", "Requiered but not used");
            data.Add("oauth_token", _accessToken);
            data.Add("oauth_version", "1.0");

            data.Add("oauth_signature", GenerateSignature(fullUrl, data));

            string oAuthHeader = GenerateOAuthHeader(data);

            var formData = new FormUrlEncodedContent(data.Where(x => !x.Key.StartsWith("oauth_")));

            return SendRequest(fullUrl, oAuthHeader, formData);
        }

        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + String.Join(
                ", ",
                data.Where(x => x.Key.StartsWith("oauth_"))
                .Select(x => $"{Uri.EscapeDataString(x.Key)}=\"{Uri.EscapeDataString(x.Value)}\"")
                .OrderBy(s => s));
        }

        private async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                var httpResp = await http.PostAsync(fullUrl, formData);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }

        private string GenerateSignature(string fullUrl, Dictionary<string, string> data)
        {
            var sigString = String.Join(
                "&",
                data.Union(data)
                .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}")
                .OrderBy(s => s));
            var fullSigString = $"POST&{Uri.EscapeDataString(fullUrl)}&{Uri.EscapeDataString(sigString.ToString())}";

            return Convert.ToBase64String(_sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigString.ToString())));
        }
    }
}