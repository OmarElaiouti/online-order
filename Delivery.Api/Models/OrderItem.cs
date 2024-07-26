namespace Delivery.Api.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int MenuItemId { get; set; }
        public int OrderId { get; set; }
        public MenuItem MenuItem { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
