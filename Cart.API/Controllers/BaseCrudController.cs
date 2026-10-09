using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto, TKey>(
    ICrudService<TEntity, TDto, TCreateDto, TUpdateDto, TKey> service)
    : ControllerBase
    where TEntity : BaseEntity<TKey>
    where TDto : BaseDto<TKey>
    where TUpdateDto : BaseDto<TKey>
    where TKey : struct
{
    protected readonly ICrudService<TEntity, TDto, TCreateDto, TUpdateDto, TKey> Service = service;

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        var result = await Service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TDto>> GetById([FromRoute] TKey id)
    {
        var result = await Service.GetByIdAsync(id);
        if (result is null)
        {
            return NotFound(new { message = $"Id: {id} olan kayıt bulunamadı." });
        }

        return Ok(result);
    }

    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto)
    {
        var created = await Service.CreateAsync(createDto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut()]
    public virtual async Task<ActionResult<TDto>> Update([FromBody] TUpdateDto updateDto)
    {
        try
        {
            var updated = await Service.UpdateAsync(updateDto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] TKey id)
    {
        var isDeleted = await Service.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound(new { message = $"Id: {id} olan kayıt bulunamadı." });
        }

        return NoContent();
    }
}