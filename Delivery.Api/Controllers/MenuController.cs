using AutoMapper;
using Delivery.Api.Dtos;
using Delivery.Api.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly RestaurantDeliveryDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        public MenuController(RestaurantDeliveryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{restaurantId}")]
        public async Task<IActionResult> GetMenuByRestaurantId(int restaurantId)
        {
            if (restaurantId <= 0)
            {
                return BadRequest("Invalid restaurant ID.");
            }

            try
            {
                var restaurant = await _context.Restaurants.FindAsync(restaurantId);
                if (restaurant == null)
                {
                    _logger.LogWarning($"Restaurant with ID {restaurantId} not found.");
                    return NotFound();
                }

                var menuItems = await _context.MenuItems
                    .Where(m => m.RestaurantId == restaurantId)
                    .Include(mi => mi.Restaurant)
                    .ThenInclude(r => r.City)
                    .ToListAsync();

                var menuToSend = _mapper.Map<List<MenuItemDto>>(menuItems);
                menuToSend.ForEach(mi => mi.Restaurantcity = restaurant.City.CityId);

                return Ok(menuToSend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the menu.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpGet("byId")]
        public async Task<IActionResult> GetMenuItemById(int ItemId)
        {
            if (ItemId <= 0)
            {
                return BadRequest("Invalid item ID.");
            }
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(ItemId);

                var menuItemDto = _mapper.Map<MenuItemDto>(menuItem);

                return Ok(menuItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the menu item.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
