using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
         // đọc dữ liệu từ trong databse
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() {
            // tham chiếu đến Products nằm trong Application DBContext. ToList() ép dữ liệu trả về thành danh sách
            var products = _context.Products.ToList()
                .Select(s => s.ToProductDto());  // select là delegate
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var product = _context.Products.Find(id); // Find này là phương thức có sẵn

            if(product == null) {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequestDto productDto) {
            var productModel = productDto.ToProductFromCreateDTO();
            _context.Products.Add(productModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new {id = productModel.Id}, productModel.ToProductDto());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto) {
            var productModel = _context.Products.FirstOrDefault(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            productModel.Name = updateDto.Name;
            productModel.Price = updateDto.Price;
            productModel.Thumbnail = updateDto.Thumbnail;
            productModel.Description = updateDto.Description;

            _context.SaveChanges();
            return Ok(productModel.ToProductDto());
        }

        [HttpDelete]
        [Route("HardDelete/{id}")]
        public IActionResult Delete([FromRoute] int id) {
            var productModel = _context.Products.FirstOrDefault(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            _context.Products.Remove(productModel);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("SoftDelete/{id}")]
        public IActionResult SoftDelete([FromRoute] int id) {
            var productModel = _context.Products.FirstOrDefault(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            // _context.Products.Remove(productModel);
            productModel.Active = false; // xóa mềm - cập nhật sản phẩm thành false
            _context.SaveChanges();
            return NoContent();
        }
    }
}