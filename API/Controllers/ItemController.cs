using API.Filters;
using Application.Abstractions;
using Application.DTOs.Item;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(LogActionFilter))]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemService _itemservice;

        public ItemController(ILogger<ItemController> logger, IItemService itemservice)
        {
            _logger = logger;
            _itemservice = itemservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _itemservice.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itemservice.GetByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ItemRequest dto)
        {
            if (dto == null) return BadRequest("Data is Not Submitted !!");

            var result = await _itemservice.CreateAsync(dto);

            if (result == null) return NotFound($"No Product found with ID {dto.ProductId}");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemRequest dto)
        {
            if (dto == null) return BadRequest();

            var result = await _itemservice.UpdateAsync(id, dto);

            if (result == null) return NotFound($"Item with ID {id} not found.");

            return Ok(result);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _itemservice.DeleteAsync(id);

            if (!success)
                return NotFound(new { Message = $"Item with ID {id} not found." });

            return NoContent();
        }
    }
}