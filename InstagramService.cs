using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ClickViajaBot.Services
{
    public static class InstagramService
    {
        public static string GetLastPostedImages()
        {
            string token = ConfigurationManager.AppSettings["instragramToken"].ToString();
            string url   = string.Format("https://api.instagram.com/v1/users/self/media/recent/?access_token=" + token);

            var client = new HttpClient();
            var rslt   = client.GetAsync(url).Result;
            var result = rslt.Content.ReadAsStringAsync().Result;

            return result;
        }

    }
}