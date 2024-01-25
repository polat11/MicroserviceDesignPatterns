using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer;
using Order.API.Model;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddMassTransit(x => {
    x.AddConsumer<PaymentSuccessEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.AddConsumer<StockNotReservedEventConsumer>();
    x.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQConnectionString"));
        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentSuccessEventQueueName, c =>
        {
            c.ConfigureConsumer<PaymentSuccessEventConsumer>(context);
        });
        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentFailedEventQueueName, c =>
        {
            c.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderStockNotReservedEventQueueName, c =>
        {
            c.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString"));
});

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
