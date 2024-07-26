using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Restaurant_delivery_online.CU;
using Restaurant_delivery_online.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_delivery_online.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IDistributedCache _cache;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IHttpClientFactory clientFactory, IDistributedCache cache, IOptions<ApiSettings> apiSettings, ILogger<OrderController> logger)
        {
            _clientFactory = clientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Confirm(List<int> selectedItems, int restaurantId)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return BadRequest("Please select at least one item to continue.");
            }

            TempData["SelectedItems"] = JsonConvert.SerializeObject(selectedItems);
            TempData["RestaurantId"] = restaurantId;
            _cache.SetString("SelectedItems", JsonConvert.SerializeObject(selectedItems));
            _cache.SetString("RestaurantId", restaurantId.ToString());

            return View("CustomerDetails");
        }

        [HttpGet]
        public IActionResult Confirm()
        {
            var selectedItemsJson = _cache.GetString("SelectedItems");
            var restaurantIdString = _cache.GetString("RestaurantId");

            if (string.IsNullOrEmpty(selectedItemsJson) || string.IsNullOrEmpty(restaurantIdString))
            {
                _logger.LogWarning("No data in the cache for selected items or restaurant ID.");
                return RedirectToAction("Index", "Home");
            }

            var selectedItems = JsonConvert.DeserializeObject<List<int>>(selectedItemsJson);
            var restaurantId = int.Parse(restaurantIdString);

            TempData["SelectedItems"] = selectedItemsJson;
            TempData["RestaurantId"] = restaurantId;

            return RedirectToAction("CustomerDetails");
        }

        [HttpGet]
        public IActionResult CustomerDetails()
        {
            var selectedItemsJson = _cache.GetString("SelectedItems");
            var restaurantIdString = _cache.GetString("RestaurantId");

            if (string.IsNullOrEmpty(selectedItemsJson) || string.IsNullOrEmpty(restaurantIdString))
            {
                _logger.LogWarning("No data in the cache for selected items or restaurant ID.");
                return RedirectToAction("Index", "Home");
            }

            TempData["SelectedItems"] = selectedItemsJson;
            TempData["RestaurantId"] = int.Parse(restaurantIdString);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerDetails(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            var selectedItemsJson = TempData["SelectedItems"] as string ?? _cache.GetString("SelectedItems");
            var selectedItemIds = JsonConvert.DeserializeObject<List<int>>(selectedItemsJson);

            if (selectedItemIds == null || !selectedItemIds.Any())
            {
                ModelState.AddModelError(string.Empty, "No items selected.");
                return View(order);
            }

            var restaurantId = TempData["RestaurantId"] as int? ?? int.Parse(_cache.GetString("RestaurantId"));

            var client = _clientFactory.CreateClient();
            var itemsDetails = new List<MenuItem>();

            foreach (var itemId in selectedItemIds)
            {
                var response = await client.GetAsync($"{_apiSettings.BaseUrl}/api/menu/byId?itemId={itemId}");
                if (response.IsSuccessStatusCode)
                {
                    var item = await response.Content.ReadAsAsync<MenuItem>();
                    itemsDetails.Add(item);
                }
                else
                {
                    _logger.LogError($"Failed to retrieve details for item ID {itemId}. Status Code: {response.StatusCode}");
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve details for item ID {itemId}.");
                    return View(order);
                }
            }

            TempData["CustomerName"] = order.CustomerName;
            TempData["CustomerEmail"] = order.CustomerEmail;
            TempData["CustomerPhone"] = order.CustomerPhone;
            TempData["CustomerAddress"] = order.CustomerAddress;
            TempData["ItemsDetails"] = JsonConvert.SerializeObject(itemsDetails);
            _cache.SetString("ItemsDetails", JsonConvert.SerializeObject(itemsDetails));

            return RedirectToAction("OrderConfirmation");
        }

        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            var customerName = TempData["CustomerName"] as string;
            var customerEmail = TempData["CustomerEmail"] as string;
            var customerPhone = TempData["CustomerPhone"] as string;
            var customerAddress = TempData["CustomerAddress"] as string;
            var itemsDetailsJson = TempData["ItemsDetails"] as string;

            if (string.IsNullOrEmpty(itemsDetailsJson))
            {
                itemsDetailsJson = _cache.GetString("ItemsDetails");
            }

            var itemsDetails = JsonConvert.DeserializeObject<List<MenuItem>>(itemsDetailsJson);

            ViewBag.CustomerName = customerName;
            ViewBag.CustomerEmail = customerEmail;
            ViewBag.CustomerPhone = customerPhone;
            ViewBag.CustomerAddress = customerAddress;
            ViewBag.ItemsDetails = itemsDetails;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] Order order)
        {
            var apiUrl = $"{_apiSettings.BaseUrl}/api/order/confirm";

            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<OrderConfirmationResult>(apiResponse);

                return Json(new
                {
                    success = true,
                    customerName = apiResult.CustomerName,
                    orderId = apiResult.OrderId,
                    totalAmount = apiResult.TotalAmount,
                    orderItems = apiResult.OrderItems
                });
            }
            else
            {
                _logger.LogError($"Failed to submit order. Status Code: {response.StatusCode}");
                return Json(new { success = false, message = "Failed to submit order. Please try again." });
            }
        }
    }
}
