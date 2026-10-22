using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ocr_receipt.Models;

namespace ocr_receipt.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReceiptsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var all = await _context.Receipts
            .Include(r => r.Products)
            .ToListAsync();
        return Ok(all);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var one = await _context.Receipts
            .Include(r => r.Products)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (one is null) return NotFound();
        return Ok(one);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReceiptDTO dto)
    {
        if (!DateOnly.TryParse(dto.BuyDate, out var buyDate))
        {
            return BadRequest("Invalid buy_date format. Expected format: YYYY-MM-DD.");
        }

        Receipt receipt = new()
        {
            StoreName = dto.StoreName,
            BuyDate = buyDate,
            TotalPrice = dto.TotalPrice,
            AmountPaid = dto.AmountPaid,
            PaymentMethod = dto.PaymentMethod,
            Changes = dto.Changes,
            Products = dto.Products?.Select(p => new Product
            {
                Name = p.ProductName,
                Quantity = p.Quantity,
                PricePerItem = p.PricePerItem
            }).ToList() ?? []
        };

        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] ReceiptDTO dto)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Products)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt is null) return NotFound();

        if (!DateOnly.TryParse(dto.BuyDate, out var buyDate))
        {
            return BadRequest("Invalid buy_date format. Expected format: YYYY-MM-DD.");
        }

        receipt.StoreName = dto.StoreName;
        receipt.BuyDate = buyDate;
        receipt.TotalPrice = dto.TotalPrice;
        receipt.AmountPaid = dto.AmountPaid;
        receipt.PaymentMethod = dto.PaymentMethod;
        receipt.Changes = dto.Changes;

        receipt.Products.Clear();
        if (dto.Products is not null)
        {
            foreach (var p in dto.Products)
            {
                receipt.Products.Add(new Product
                {
                    Name = p.ProductName,
                    Quantity = p.Quantity,
                    PricePerItem = p.PricePerItem
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(receipt);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] ReceiptPatchDTO dto)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Products)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt is null) return NotFound();

        if (dto.StoreName is not null)
        {
            receipt.StoreName = dto.StoreName;
        }

        if (dto.BuyDate is not null)
        {
            if (!DateOnly.TryParse(dto.BuyDate, out var buyDate))
            {
                return BadRequest("Invalid buy_date format. Expected format: YYYY-MM-DD.");
            }
            receipt.BuyDate = buyDate;
        }

        if (dto.TotalPrice.HasValue)
        {
            receipt.TotalPrice = dto.TotalPrice.Value;
        }

        if (dto.AmountPaid.HasValue)
        {
            receipt.AmountPaid = dto.AmountPaid.Value;
        }

        if (dto.PaymentMethod is not null)
        {
            receipt.PaymentMethod = dto.PaymentMethod;
        }

        if (dto.Changes.HasValue)
        {
            receipt.Changes = dto.Changes.Value;
        }

        if (dto.Products is not null)
        {
            receipt.Products.Clear();
            foreach (var p in dto.Products)
            {
                receipt.Products.Add(new Product
                {
                    Name = p.ProductName,
                    Quantity = p.Quantity,
                    PricePerItem = p.PricePerItem
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(receipt);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var receiptToDelete = await _context.Receipts
            .Include(r => r.Products)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receiptToDelete is null) return NotFound();

        _context.Receipts.Remove(receiptToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
