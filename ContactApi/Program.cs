using ContactApi.Data;
using ContactApi.Filters;
using ContactApi.V1.Models.Requests;
using ContactApi.V1.Models.Requests.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(options => { options.ReportApiVersions = true; });

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers(options => options.Filters.Add(new GlobalExceptionFilter()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact API", Version = "v1" });

    c.TagActionsBy(api =>
    {
        if(api != null)
        {
            return new[] { api.GroupName };
        }

        if(api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint");
    });

    c.DocInclusionPredicate((_, _) => true);
});

builder.Services.AddScoped<IValidator<CreateContactRequestModel>, CreateContactRequestModelValidator>();
builder.Services.AddScoped<IValidator<UpdateContactRequestModel>, UpdateContactRequestModelValidator>();

builder.Services.AddDbContext<ContactDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("ContactDb")));

var app = builder.Build();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ContactDbContext>();
context.Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
