namespace Restaurant_delivery_online.Models
{
    public class OrderConfirmationResult
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDetails> OrderItems { get; set; }


    }

}
