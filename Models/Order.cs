using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Quantity cannot be empty or zero")]
        public decimal Quantity { get; set; }
    }
}
