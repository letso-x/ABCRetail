using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Order
    {
        public string OrderId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public string ProductId { get; set; } = "";
        public int Quantity { get; set; }
    }
}
