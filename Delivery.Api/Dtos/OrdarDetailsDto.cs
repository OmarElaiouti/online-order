namespace Delivery.Api.Dtos
{
    public class OrdarDetailsDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrdarItemDetailsDto> OrderItems { get; set; }
    }
}
