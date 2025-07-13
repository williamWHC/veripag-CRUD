using Orders.Application.Interfaces;
using Orders.Infra.Repositories;
using Orders.Infra.Services;
using Orders.API.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Orders API",
        Version = "v1",
        Description = "API para gerenciamento de pedidos"
    });

    // Configurar documentação XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Dependency Injection
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

// Middleware de tratamento de erros
app.UseErrorHandling();

app.UseAuthorization();

app.MapControllers();

app.Run();
