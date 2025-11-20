using MongoDB.Driver;
using PropertyAPI.Application.Interfaces;
using PropertyAPI.Application.Mappings;
using PropertyAPI.Application.Services;
using PropertyAPI.Domain.Interfaces;
using PropertyAPI.Infrastructure.Data;
using PropertyAPI.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuración de MongoDB
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") 
    ?? throw new InvalidOperationException("MongoDB connection string is not configured.");
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] 
    ?? throw new InvalidOperationException("MongoDB database name is not configured.");

// Log para depuración
var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("MongoDB");
logger.LogInformation("=== CONFIGURACIÓN DE MONGODB ===");
logger.LogInformation("Connection String (masked): {ConnectionString}", 
    mongoConnectionString.Substring(0, Math.Min(50, mongoConnectionString.Length)) + "...");
logger.LogInformation("Database Name: {DatabaseName}", mongoDatabaseName);
logger.LogInformation("Environment: {Environment}", builder.Environment.EnvironmentName);

var mongoClient = new MongoClient(mongoConnectionString);

// Obtener información del servidor antes de la conexión
var serverAddress = mongoClient.Settings.Servers?.FirstOrDefault();
if (serverAddress != null)
{
    logger.LogInformation("Servidor configurado: {Host}:{Port}", serverAddress.Host, serverAddress.Port);
}

// Verificar información de conexión
try
{
    // Obtener información del cluster
    var cluster = mongoClient.Cluster;
    logger.LogInformation("Cluster State: {State}", cluster.Description.State);
    
    // Verificar si la cadena de conexión es de Atlas
    var isAtlasConnectionString = mongoConnectionString.Contains("mongodb.net") || mongoConnectionString.Contains("mongodb+srv://");
    logger.LogInformation("¿Cadena de conexión es de Atlas? {IsAtlas}", isAtlasConnectionString);
    
    // Obtener información de los servidores configurados
    if (mongoClient.Settings?.Servers != null)
    {
        foreach (var server in mongoClient.Settings.Servers)
        {
            logger.LogInformation("Servidor configurado: {Host}:{Port}", server.Host, server.Port);
            
            // Verificar si es localhost (MongoDB local)
            var isLocal = server.Host == "localhost" || server.Host == "127.0.0.1";
            if (isLocal && isAtlasConnectionString)
            {
                logger.LogWarning("⚠️ ADVERTENCIA: La cadena de conexión indica Atlas, pero el servidor configurado es MongoDB local ({Host}:{Port})!", server.Host, server.Port);
                logger.LogWarning("⚠️ Esto sugiere que el driver puede estar resolviendo mal la cadena de conexión o hay un problema con DNS.");
            }
        }
    }
    
    logger.LogInformation("✅ Cliente MongoDB creado exitosamente");
}
catch (Exception ex)
{
    logger.LogError(ex, "❌ Error al crear cliente MongoDB: {Message}", ex.Message);
    throw;
}

var mongoContext = new MongoDbContext(mongoClient, mongoDatabaseName);

// Registrar servicios
builder.Services.AddSingleton(mongoContext);
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();


builder.Services.AddAutoMapper(typeof(PropertyMappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Property API",
        Version = "v1",
        Description = "API para gestionar y consultar propiedades con filtros avanzados",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Property API Support",
            Email = "support@propertyapi.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))   
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Property API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

