using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ocr_receipt.Models;

namespace ocr_receipt.Controllers;

[ApiController]
[Route("api/v1/ocr")]
public class OCRProcessesController(AppDbContext context, ILogger<OCRProcessesController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<OCRProcessesController> _logger = logger;

    [HttpGet]
    [Tags("GetAllData")]
    public async Task<IActionResult> Get()
    {
        var data = await _context.OCRProcesses.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetImage([FromRoute] int id)
    {
        var item = await _context.OCRProcesses.FindAsync(id);
        if (item == null || string.IsNullOrEmpty(item.ImagePath))
            return NotFound();
        // 1. Extract the file name (e.g. 9613318a-62cd-4401-9a67-eca48f858008.jpeg)
        var fileName = Path.GetFileName(item.ImagePath);
        // 2. Resolve to the path inside the Docker container
        var containerPath = Path.Combine("/Uploads", fileName);
        if (System.IO.File.Exists(containerPath))
        {
            return PhysicalFile(containerPath, "image/jpeg");
        }
        // 3. Fallback check for running directly on host without Docker
        if (System.IO.File.Exists(item.ImagePath))
        {
            return PhysicalFile(item.ImagePath, "image/jpeg");
        }
        return NotFound();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        OCRProcess? byid = await _context.OCRProcesses.FindAsync(id);
        if (byid is null) return NotFound();
        return Ok(byid);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] IFormFile File)
    {
        if (File == null || File.Length == 0) return BadRequest();
        string[] AllowedExt = [".jpeg", ".jpg", ".png", ".svg"];
        string Extensions = Path.GetExtension(File.FileName).ToLowerInvariant();

        if (!AllowedExt.Contains(Extensions)) return BadRequest("Invalid types");

        try
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFilename = $"{Guid.NewGuid()}{Extensions}";
            string filePath = Path.Combine(uploadsFolder, uniqueFilename);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await File.CopyToAsync(stream);

            OCRProcess ocr = new()
            {
                ImagePath = filePath,
                IsProcessed = false,
            };

            _context.OCRProcesses.Add(ocr);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                fileName = uniqueFilename,
                message = "Image uploaded succesfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while inserting record for {FileName}", File.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error occured while inserting a record");
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] OCRProcessDTO dto)
    {
        OCRProcess? result = await _context.OCRProcesses.FindAsync(id);
        if (result is null) return NotFound();
        try
        {
            if(dto.IsProcessed.HasValue) result.IsProcessed = dto.IsProcessed.Value;
            if (!string.IsNullOrWhiteSpace(dto.ImagePath)) result.ImagePath = dto.ImagePath;
            if (dto.JSONText != null) result.JSONText = dto.JSONText;
            if (dto.RawTextPath != null) result.RawTextPath = dto.RawTextPath;

            await _context.SaveChangesAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while updating a record for id: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error occured while updating the record");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            OCRProcess? result = await _context.OCRProcesses.FindAsync(id);
            if (result is null) return NotFound();
            string? imagePath = result.ImagePath;
            _context.OCRProcesses.Remove(result);

            await _context.SaveChangesAsync();
            if (!string.IsNullOrWhiteSpace(imagePath) && System.IO.File.Exists(imagePath))
            {
                try
                {
                    System.IO.File.Delete(imagePath);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to delete physical file image for ocr process: {Id}", id);
                }
            }
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while deleting a record with id: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error occured while deleting a record");
        }
    }
}
