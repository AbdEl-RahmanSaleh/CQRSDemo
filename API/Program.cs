using API.Exceptions;
using Application.Common.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// register mediatR services from the Application assembly
builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Application.IAssemblyMarker).Assembly));

// register fluent validation services from the Application assembly
builder.Services.AddValidatorsFromAssembly(typeof(Application.IAssemblyMarker).Assembly);

// register the validation behavior for MediatR with any request and response types
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Application.Behaviors.ValidationBehavior<,>));

builder.Services.AddScoped<IAppDbContext,AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
