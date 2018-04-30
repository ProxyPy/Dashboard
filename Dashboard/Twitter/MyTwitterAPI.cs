using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Dashboard;

namespace Dashboard.API.Twitter
{
    public class MyTwitterAPI
    {
        string _baseUrl = "https://api.twitter.com/1.1/";
        string _consumerKey = "k3yROJtZxNoF64VwaoJ4q0hm1"; // The application's consumer key
        //string _consumerSecret;
        string _accessToken = "985079514886754305-J3E8efwvfCLU3T4nKL6EgpIgUY8rPwY"; // The access token granted after OAuth authorization
        //string _accessTokenSecret;
        readonly HMACSHA1 _sigHasher;
        DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        HttpClient _client = new HttpClient();

        public MyTwitterAPI(string consumerSecret, string accessTokenSecret)
        {
            _sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes($"{consumerSecret}&{accessTokenSecret}"));
        }

        public string GetTimeline()
        {
            var url = "statuses/home_timeline.json";
            var data = MakeOAuthHeaders(url);
            var headers = "OAuth " + String.Join(",", data.Select(x => $"{Uri.EscapeDataString(x.Key)}=\"{Uri.EscapeDataString(x.Value)}\""));

            //return headers;
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Add("Authorization", "OAuth oauth_consumer_key=\"k3yROJtZxNoF64VwaoJ4q0hm1\",oauth_token=\"985079514886754305-7QYBJMoqvSBdwHCyxHKSNJM21PZxxWj\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"1525117182\",oauth_nonce=\"z\",oauth_version=\"1.0\",oauth_signature=\"h2eV3ALKWsthy6lBIPnU3SV57T8%3D\"");
                client.DefaultRequestHeaders.Add("Authorization", headers);

                var response = client.GetAsync(_baseUrl + url);
                response.Wait();
                var responseBody = response.Result.Content.ReadAsStringAsync();
                responseBody.Wait();

                return responseBody.Result;
            }
        }

        private Dictionary<string, string> MakeOAuthHeaders(string url)
        {
            var data = new Dictionary<string, string>();
            var fullUrl = _baseUrl + url;

            var timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);

            data.Add("oauth_consumer_key", _consumerKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            //data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_timestamp", "1525118787");
            data.Add("oauth_nonce", "z");
            data.Add("oauth_token", _accessToken);
            data.Add("oauth_version", "1.0");

            data.Add("oauth_signature", GenerateSignature("GET", fullUrl, data));

            //string oAuthHeader = GenerateOAuthHeader(data);

            //var formData = new FormUrlEncodedContent(data.Where(x => !x.Key.StartsWith("oauth_")));

            return data;
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

            data.Add("oauth_signature", GenerateSignature("POST", fullUrl, data));


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

        private string GenerateSignature(string method, string fullUrl, Dictionary<string, string> data)
        {
            var sigString = String.Join(
                "&",
                data.Union(data)
                .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}")
                .OrderBy(s => s));
            var fullSigString = $"{method}&{Uri.EscapeDataString(fullUrl)}&{Uri.EscapeDataString(sigString.ToString())}";

            return Convert.ToBase64String(_sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigString.ToString())));
        }
    }
}