namespace Delivery.Api.Dtos
{
    public class OrdarItemDetailsDto
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public string RestaurantName { get; set; }
    }
}
