namespace Delivery.Api.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string RestaurantImage { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }

    }

}
