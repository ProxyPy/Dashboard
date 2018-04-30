using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard
{
    public class MyHttpHelper
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