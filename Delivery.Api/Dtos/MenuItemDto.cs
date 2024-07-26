namespace Delivery.Api.Dtos
{
    public class MenuItemDto
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int RestaurantId { get; set; }
        public int Restaurantcity { get; set; }
        public string MenuItemImage { get; set; }


    }
}
