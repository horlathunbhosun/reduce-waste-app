using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionDynamicColumnController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionDynamicColumnController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/TransactionDynamicColumn
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionColumnDefinitionDto>>> GetColumnDefinitions()
    {
        var columns = await _context.TransactionColumnDefinitions
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var columnDtos = columns.Select(c => new TransactionColumnDefinitionDto
        {
            Id = c.Id,
            Name = c.Name,
            DisplayName = c.DisplayName,
            Type = c.Type,
            Required = c.Required,
            DefaultValue = c.DefaultValue,
            Options = string.IsNullOrEmpty(c.Options) ? new Dictionary<string, object>() : 
                     JsonSerializer.Deserialize<Dictionary<string, object>>(c.Options) ?? new Dictionary<string, object>(),
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(columnDtos);
    }

    // POST: api/TransactionDynamicColumn
    [HttpPost]
    public async Task<ActionResult<TransactionColumnDefinitionDto>> AddColumn(AddTransactionColumnDto dto)
    {
        // Check if column name already exists
        var existingColumn = await _context.TransactionColumnDefinitions
            .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.ToLower());

        if (existingColumn != null)
        {
            return BadRequest("A column with this name already exists.");
        }

        // Validate column name (no spaces, special characters)
        if (string.IsNullOrWhiteSpace(dto.Name) || !IsValidColumnName(dto.Name))
        {
            return BadRequest("Column name must be a valid identifier (no spaces or special characters).");
        }

        var column = new TransactionColumnDefinition
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Type = dto.Type,
            Required = dto.Required,
            DefaultValue = dto.DefaultValue,
            Options = JsonSerializer.Serialize(dto.Options),
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TransactionColumnDefinitions.Add(column);
        await _context.SaveChangesAsync();

        var resultDto = new TransactionColumnDefinitionDto
        {
            Id = column.Id,
            Name = column.Name,
            DisplayName = column.DisplayName,
            Type = column.Type,
            Required = column.Required,
            DefaultValue = column.DefaultValue,
            Options = dto.Options,
            DisplayOrder = column.DisplayOrder,
            IsActive = column.IsActive,
            CreatedAt = column.CreatedAt,
            UpdatedAt = column.UpdatedAt
        };

        return CreatedAtAction(nameof(GetColumnDefinitions), new { id = column.Id }, resultDto);
    }

    // PUT: api/TransactionDynamicColumn/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateColumn(Guid id, UpdateTransactionColumnDto dto)
    {
        var column = await _context.TransactionColumnDefinitions.FindAsync(id);
        if (column == null)
        {
            return NotFound();
        }

        column.DisplayName = dto.DisplayName;
        column.Required = dto.Required;
        column.DefaultValue = dto.DefaultValue;
        column.Options = JsonSerializer.Serialize(dto.Options);
        column.DisplayOrder = dto.DisplayOrder;
        column.IsActive = dto.IsActive;
        column.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "Error updating column definition.");
        }

        return NoContent();
    }

    // DELETE: api/TransactionDynamicColumn/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteColumn(Guid id)
    {
        var column = await _context.TransactionColumnDefinitions.FindAsync(id);
        if (column == null)
        {
            return NotFound();
        }

        // Soft delete - just mark as inactive
        column.IsActive = false;
        column.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/TransactionDynamicColumn/reorder
    [HttpPost("reorder")]
    public async Task<IActionResult> ReorderColumns(List<Guid> columnIds)
    {
        var columns = await _context.TransactionColumnDefinitions
            .Where(c => columnIds.Contains(c.Id))
            .ToListAsync();

        for (int i = 0; i < columnIds.Count; i++)
        {
            var column = columns.FirstOrDefault(c => c.Id == columnIds[i]);
            if (column != null)
            {
                column.DisplayOrder = i;
                column.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool IsValidColumnName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Check if it starts with a letter or underscore
        if (!char.IsLetter(name[0]) && name[0] != '_')
            return false;

        // Check if all characters are alphanumeric or underscore
        return name.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
} 