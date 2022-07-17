using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Common.Settings;

const string AllowedOriginSetting = "AllowedOrigin";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var rabbitMQSettings = builder.Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

ConfigureServices(builder.Services);

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(corsBuilder =>
    {
        corsBuilder
            //.WithOrigins(builder.Configuration[AllowedOriginSetting])
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    // Create Mongo DB instance with repository
    services.AddMongo().AddMongoRepository<ItemModel>("items");

    services.AddMassTransitWithRabbitMq();

    // Additional services
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}