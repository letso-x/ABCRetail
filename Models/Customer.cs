using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Customer
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage="Email address is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(15)]
        public string Phone { get; set; }
    }
}
