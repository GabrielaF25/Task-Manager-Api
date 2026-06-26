using Microsoft.EntityFrameworkCore;
using Task_Manager_Api.Middlewares;
using TaskManager.Domain.DependencyInjections;
using TaskManager.Infrastructure.Authentication;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.


        builder.Services.AddDbContext<TaskManagerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices();
        builder.Services.AddExceptionHandler<GlobalExceptionMiddleware>();
        builder.Services.AddProblemDetails();
        builder.Services.AddOptions<JwtSettings>()
                                .BindConfiguration(JwtSettings.SectionName)
                                .ValidateDataAnnotations()
                                .ValidateOnStart();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

 
        app.MapControllers();

        app.Run();
    }
}