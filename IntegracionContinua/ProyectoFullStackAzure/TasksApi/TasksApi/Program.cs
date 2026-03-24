
using TasksApi.Services;

var builder = WebApplication.CreateBuilder(args);

//Añadir soporte para controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Servicio Cosmos
builder.Services.AddSingleton<CosmosService>();

var app = builder.Build();

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// MAPEAR CONTROLLERS
app.MapControllers();

app.Run();
