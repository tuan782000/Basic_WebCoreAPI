using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetAll() {
            // tham chiếu đến Products nằm trong Application DBContext. ToList() ép dữ liệu trả về thành danh sách
            var products = await _context.Products.ToListAsync(); // này là lấy ra danh sách sản phẩm
            var productDto = products.Select(s => s.ToProductDto());  // select là delegate
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var product = await _context.Products.FindAsync(id); // Find này là phương thức có sẵn

            if(product == null) {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto productDto) {
            var productModel = productDto.ToProductFromCreateDTO();
            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = productModel.Id}, productModel.ToProductDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto) {
            var productModel = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            productModel.Name = updateDto.Name;
            productModel.Price = updateDto.Price;
            productModel.Thumbnail = updateDto.Thumbnail;
            productModel.Description = updateDto.Description;

            await _context.SaveChangesAsync();
            return Ok(productModel.ToProductDto());
        }

        [HttpDelete]
        [Route("HardDelete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id) {
            var productModel = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            /*
            The reason why Delete() does not allow await is:
            - It does not involve any immediate database interaction when called, merely a state change in memory
            - does not involve waiting and is a quick in-memory operation, making it asynchronous would not provide any benefits and could even lead to less efficient resource utilization.

            Thank you for this amazing content, + leaving a question at the end is a good idea it make us dig more into .NET tricks, keep up the good work
            */
            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete]
        [Route("SoftDelete/{id}")]
        public async Task<IActionResult> SoftDelete([FromRoute] int id) {
            var productModel = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(productModel == null) {
                return NotFound();
            }

            // _context.Products.Remove(productModel);
            productModel.Active = false; // xóa mềm - cập nhật sản phẩm thành false
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}