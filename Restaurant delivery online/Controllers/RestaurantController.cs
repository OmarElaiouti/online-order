using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Restaurant_delivery_online.CU;
using Restaurant_delivery_online.Models;
using System.Net.Http;

namespace Restaurant_delivery_online.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestaurantController> _logger;
        private readonly ApiSettings _apiSettings;

        public RestaurantController(HttpClient httpClient, ILogger<RestaurantController> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apiSettings.Value;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(int cityId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_apiSettings.BaseUrl}/api/Restaurants/bycity?cityId={cityId}");
            var restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(response);
            return View(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching restaurants for cityId: {CityId}", cityId);
                return RedirectToAction("ServerErrorPage", "Home");
            }
        }
    }
}
