using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IProductRepository _productRepo;
        public ProductController(ApplicationDBContext context, IProductRepository productRepo)
        {
            _context = context;
            _productRepo = productRepo;
        }
        // khi mà để Authorize - chỉ có đăng nhập mới dùng được
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query) {

            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // tham chiếu đến Products nằm trong Application DBContext. ToList() ép dữ liệu trả về thành danh sách
            // var products = await _context.Products.ToListAsync(); // này là lấy ra danh sách sản phẩm

            // Thay vì phải dựa vào  _context chọc vào Products lấy ra hàm ToListAsync. Thì giờ đây có thể lấu thông qua Repository - ProductRepository của IProductRepository

            var products = await _productRepo.GetAllAsync(query); // mọi xử lý đều được viết trong hàm GetAllAsync

            var productDto = products.Select(s => s.ToProductDto());  // select là delegate
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var product = await _productRepo.GetByIdAsync(id); 

            if(product == null) {
                return NotFound();
            }

            return Ok(product.ToProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto productDto) {
            
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productModel = productDto.ToProductFromCreateDTO();
            // Thay vì dùng này
            // await _context.Products.AddAsync(productModel);
            // await _context.SaveChangesAsync();
            // đã viết nó vào trong ProductRepository và chỉ việc tham chiếu đến IProductRepository để lấy "CreateAsync" ra dùng
            await _productRepo.CreateAsync(productModel);
            return CreatedAtAction(nameof(GetById), new {id = productModel.Id}, productModel.ToProductDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequestDto updateDto) {

            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productModel = await _productRepo.UpdateAsync(id, updateDto);

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
        [Route("HardDelete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id) {

            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productModel = await _productRepo.DeleteAsync(id);

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
        [Route("SoftDelete/{id:int}")]
        public async Task<IActionResult> SoftDelete([FromRoute] int id) {

            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productModel = await _productRepo.SoftDeleteAsync(id);

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