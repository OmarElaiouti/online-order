using System.ComponentModel.DataAnnotations;

namespace Restaurant_delivery_online.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name cannot contain numbers.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address.")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid phone number")]

        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string CustomerAddress { get; set; }

        public List<OrderItem>? OrderItems { get; set; }

    }
}
