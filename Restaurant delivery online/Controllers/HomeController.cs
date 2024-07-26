using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Restaurant_delivery_online.CU;
using Restaurant_delivery_online.Models;
using System.Diagnostics;
using System.Net.Http;

namespace Restaurant_delivery_online.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.GetStringAsync($"{_apiSettings.BaseUrl}/api/restaurants/cities");
            if (string.IsNullOrEmpty(response))
            {
                _logger.LogWarning("Received empty response from the API.");
                return View("ServerErrorPage");
            }
                var cities = JsonConvert.DeserializeObject<List<City>>(response);

                if (cities == null)
                {
                    _logger.LogError("Failed to deserialize cities from API response.");
                    return View("ServerErrorPage");
                }

                return View(cities);
            }
            catch (HttpRequestException httpRequestException)
            {
                _logger.LogError(httpRequestException, "An error occurred while calling the API.");
                return View("ServerErrorPage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return View("ServerErrorPage");
            }
        }

        public IActionResult ServerErrorPage()
        {
            return View();
        }
        
        
        public IActionResult NotFoundPage()
        {
            return View();
        }

       
    }
}
