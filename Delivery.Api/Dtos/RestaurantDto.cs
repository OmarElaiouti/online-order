using Delivery.Api.Models;

namespace Delivery.Api.Dtos
{
    public class RestaurantDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public string RestaurantImage { get; set; }

    }
}
