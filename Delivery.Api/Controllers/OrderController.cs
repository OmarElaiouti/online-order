using Delivery.Api.Models.Context;
using Delivery.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Delivery.Api.Dtos;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RestaurantDeliveryDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger; 

        public OrderController(RestaurantDeliveryDbContext context, IMapper mapper, ILogger<OrderController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                var newOrder = _mapper.Map<Order>(orderDto);

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                var orderItems = await _context.OrderItems
                            .Include(oi => oi.MenuItem)
                            .ThenInclude(mi => mi.Restaurant)
                            .Where(o=>o.OrderId== newOrder.OrderId)
                            .ToListAsync();

            

                var detailedOrderResponse = new OrdarDetailsDto
                {
                    OrderId = newOrder.OrderId,
                    CustomerName = newOrder.CustomerName,
                    TotalAmount = orderItems.Sum(oi => oi.TotalPrice),
                    OrderItems = orderItems.Select(oi => new OrdarItemDetailsDto
                    {
                        OrderItemId = oi.OrderItemId,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.TotalPrice,
                        MenuItemName = oi.MenuItem.Name,
                        MenuItemPrice = oi.MenuItem.Price,
                        RestaurantName = oi.MenuItem.Restaurant.Name
                    }).ToList()
                };

                return Ok(detailedOrderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the order.");

                return StatusCode(500, "Internal server error occurred while processing the order.");
            }
        }

    }
}
