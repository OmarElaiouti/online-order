namespace Restaurant_delivery_online.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string MenuItemImage { get; set; }

        public int RestaurantId { get; set; }
        public int Restaurantcity { get; set; }

    }
}
