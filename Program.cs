using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
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
        builder.Services.AddOpenApi();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        // Configure the HTTP request pipeline.

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
