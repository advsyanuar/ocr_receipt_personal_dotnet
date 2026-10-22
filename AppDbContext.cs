using Microsoft.EntityFrameworkCore;
using ocr_receipt.Models;

namespace ocr_receipt;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<OCRProcess> OCRProcesses { get; set; }

}
