using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TicTacToe.Api.Data;
using TicTacToe.Api.Extensions;
using TicTacToe.Api.Interfaces;
using TicTacToe.Api.Services;
using Swashbuckle.AspNetCore;

namespace TicTacToe.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(8080);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Tic Tac Toe API",
                    Description = "REST API для игры в крестики-нолики NxN"
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IGameService, GameService>();

            var app = builder.Build();

            app.Logger.LogInformation("Application started on port 8080");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tic Tac Toe API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
