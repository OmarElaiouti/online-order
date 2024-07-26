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
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantDeliveryDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(RestaurantDeliveryDbContext context, IMapper mapper, ILogger<RestaurantsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet("bycity")]
        public async Task<IActionResult> GetRestaurantsByCity(int cityId)
        {
            if (cityId <= 0)
            {
                return BadRequest("Invalid city ID.");
            }

            try
            {
                var restaurants = await _context.Restaurants
                    .Where(r => r.CityId == cityId)
                    .ToListAsync();

                if (restaurants == null || !restaurants.Any())
                {
                    _logger.LogWarning($"No restaurants found in city with ID {cityId}.");
                    return NotFound();
                }

                var restaurantsFound = _mapper.Map<List<RestaurantDto>>(restaurants);
                return Ok(restaurantsFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting restaurants by city.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Cities 
                .ToListAsync();
            var citiesFound = _mapper.Map<List<CityDto>>(cities);

            return Ok(citiesFound);
        }


    }
}
