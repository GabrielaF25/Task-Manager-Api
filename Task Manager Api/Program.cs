using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Task_Manager_Api.DbContexts;
using Task_Manager_Api.Profiles;
using Task_Manager_Api.Repository;
using Task_Manager_Api.Services;
using Task_Manager_Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());
builder.Services.AddValidatorsFromAssemblyContaining<TodoRequestValidation>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<TaskManagerDb>( options => 
options.UseSqlServer( builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
