using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
        public string ImageName { get; set; }
    }
}
