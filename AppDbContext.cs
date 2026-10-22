using Microsoft.EntityFrameworkCore;
using ocr_receipt.Models;

namespace ocr_receipt;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<OCRProcess> OCRProcesses { get; set; }

}
