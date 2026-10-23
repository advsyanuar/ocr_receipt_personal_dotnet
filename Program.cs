using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("AllowFrontendLocal", pol =>
            {
                pol.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        // Add services to the container.
/*         builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["AuthConfiguration:Issuer"],
                ValidAudience = builder.Configuration["AuthConfiguration:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["AuthConfiguration:Key"]!))
            };
        }); */

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        app.UseRouting();
        // Configure the HTTP request pipeline.
        app.UseCors("AllowFrontendLocal");
        //app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
