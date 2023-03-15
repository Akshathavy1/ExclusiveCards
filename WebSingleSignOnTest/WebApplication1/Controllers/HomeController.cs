using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private string _basePath = "https://exclusivecards-test.azurewebsites.net/api/";
        private string _basePath = "https://localhost:44325/api/";
        //private string _domain = "exclusivecards-test.azurewebsites.net";
        private string _domain = "localhost:44325";


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TestSSO()
        {
            var client = new HttpClient();
            var response = client.GetAsync(_basePath + "partner/Login?userName=ConsumerRights&password=6ykRDRpkc8~_").Result;
            var token = response.Headers.GetValues("Authorization").First();
            var tokenValue = token.Substring(8);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenValue);
            response = client.GetAsync(_basePath + "partner/Customer/SignOn?customerUserName=newuser2@mctest.com").Result;
            IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            var cookie = cookies.First();

            string url = _basePath + "partner/Customer/Redirect?url=/&cookie=" + cookie + "&token=" + token;
            return new RedirectResult(url);

            
        }

    }
}
