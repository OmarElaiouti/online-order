using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Restaurant_delivery_online.CU;
using Restaurant_delivery_online.Models;
using System.Net.Http;

namespace Restaurant_delivery_online.Controllers
{
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MenuController> _logger;
        private readonly ApiSettings _apiSettings;

        public MenuController(IHttpClientFactory httpClientFactory, ILogger<MenuController> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiSettings = apiSettings.Value;

        }

        public async Task<IActionResult> Index(int restaurantId)
        {
            if(restaurantId <= 0)
            {
                _logger.LogWarning("Invalid restaurant ID provided.");
                return RedirectToAction("NotFoundPage", "Home");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.GetAsync($"{_apiSettings.BaseUrl}/api/menu/{restaurantId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to retrieve menu items for restaurant ID {restaurantId}. Status Code: {response.StatusCode}");
                    return RedirectToAction("NotFoundPage", "Home");
                }

                var json = await response.Content.ReadAsStringAsync();
                var menuItems = JsonConvert.DeserializeObject<IEnumerable<MenuItem>>(json);

                if (menuItems == null || !menuItems.Any())
                {
                    _logger.LogWarning($"No menu items found for restaurant ID {restaurantId}.");
                    return RedirectToAction("NotFoundPage", "Home");
                }

                ViewBag.RestaurantId = restaurantId;
                ViewBag.City = menuItems.FirstOrDefault()?.Restaurantcity;

                return View(menuItems);
            }
            catch (HttpRequestException httpRequestException)
            {
                _logger.LogError(httpRequestException, "An error occurred while calling the API.");
                return RedirectToAction("ServerErrorPage", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return RedirectToAction("ServerErrorPage", "Home");
            }
        }

    }
}
