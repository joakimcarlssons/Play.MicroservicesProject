using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Clients;
using Polly;
using Polly.Timeout;

const string AllowedOriginSetting = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services);

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
    services
        .AddMongo()
        .AddMongoRepository<InventoryItem>("inventoryitems")
        .AddMongoRepository<CatalogItem>("catalogitems");

    services.AddMassTransitWithRabbitMq();

    services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5001");
    })
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        retryCount: 5,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)), // Adding some randomness to avoid multiple instances waiting the exact same time
        onRetry: (outcome, timeSpan, retryApptempt) =>
        {
            Console.WriteLine($"--> Delaying for { timeSpan.TotalSeconds } seconds, then making retry { retryApptempt }");
        }
    ))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(15),
        onBreak: (outcome, timeSpan) =>
        {
            Console.WriteLine($"--> Opening the circuit for { timeSpan.TotalSeconds } seconds...");
        },
        onReset: () =>
        {
            Console.WriteLine($"--> Closing the circuit...");
        }
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1)); // Number of seconds before timing out

    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}
