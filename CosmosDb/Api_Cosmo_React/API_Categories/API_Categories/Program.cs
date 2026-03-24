using API_Categories.Model;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Category API", Version = "v1" });
});
builder.Services.AddHttpClient();


builder.Services.AddSingleton<ICategoryCosmosService>(options =>
{
    string URL = builder.Configuration.GetSection("CategoriesDB").GetValue<string>("Account");
    string primaryKey = builder.Configuration.GetSection("CategoriesDB").GetValue<string>("Key");
    string dbName = builder.Configuration.GetSection("CategoriesDB").GetValue<string>("DatabaseName");
    string containerName = builder.Configuration.GetSection("CategoriesDB").GetValue<string>("ContainerName");

    var cosmosClient = new CosmosClient(URL, primaryKey, new CosmosClientOptions()
    {
        ConnectionMode = ConnectionMode.Gateway
    });

    return new CategoryCosmosService(cosmosClient, dbName, containerName);
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Category API v1"));
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();