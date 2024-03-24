using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IProductRepository _productRepo;
        public CommentController(ICommentRepository commentRepo, IProductRepository productRepo) {
            _commentRepo = commentRepo;
            _productRepo = productRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }
        // [HttpGet]
        // [Route("{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var comment = await _commentRepo.GetByIdAsync(id);

            if(comment == null) {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost ("{productId}")]
        public async Task<IActionResult> Create([FromRoute] int productId, CreateCommentDto commentDto) {
            if(!await _productRepo.ProductExists(productId)) {
                return BadRequest(" Product Does not exists");
            }

            var commentModel = commentDto.ToCommentFromCreate(productId);
            await _commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new {id = commentModel.Id}, commentModel.ToCommentDto());
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto) {
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if (comment == null) {
                return NotFound("Comment not found");
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ([FromRoute] int id) {
          var commentModel = await _commentRepo.DeleteAsync(id);
        
            if (commentModel == null) {
                return NotFound("Comment does not exists");
            }

          return Ok(commentModel);
        }
    }
}