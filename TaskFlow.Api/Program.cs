using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Configurations;
using TaskFlow.Infrastructure.Configurations;
using TaskFlow.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

builder.Services.AddTaskFlowServices(); 
builder.Services.AddTaskFlowInfrastructure(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();