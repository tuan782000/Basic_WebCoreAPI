using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Product
{
    public class CreateProductRequestDto
    {
        [Required]
        [MinLength(10, ErrorMessage = "Name must be 5 characters")]
        [MaxLength(280, ErrorMessage = "Name must be cannot be over 280 characters")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Range(0, 1000000000, ErrorMessage = "Price must be between 0 and 1000000000")]
        public float Price { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Description must be 5 characters")]
        [MaxLength(280, ErrorMessage = "Description must be cannot be over 280 characters")]
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; } // Áp dụng xóa mềm
    }
}