namespace Restaurant_delivery_online.Models
{
    public class OrderItemDetails
    {
        public int OrderItemId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public string RestaurantName { get; set; }
    }
}
