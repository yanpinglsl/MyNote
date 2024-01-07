using System.ComponentModel.DataAnnotations;

namespace ModelValidatorDemo
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0.01, 999.99)]
        public double Price { get; set; }
    }
}
