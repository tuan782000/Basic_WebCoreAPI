using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; } // Áp dụng xóa mềm

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}