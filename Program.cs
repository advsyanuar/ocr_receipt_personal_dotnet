using Microsoft.EntityFrameworkCore;

namespace ocr_receipt;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        bool isdev = builder.Environment.IsDevelopment();
        string? connectionString = isdev ? builder.Configuration.GetConnectionString("DefaultConnection") : builder.Configuration.GetConnectionString("Connection");
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));
        // Add services to the container.
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
