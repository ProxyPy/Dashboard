using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Dashboard
{
    public class MyInternetHelper
    {
        public static string GetResposeFromUrl(string url, HttpClient client)
        {
            var response = client.GetAsync(url);
            response.Wait();
            var readTask = response.Result.Content.ReadAsStringAsync();
            readTask.Wait();

            return readTask.Result;
        }
    }
}