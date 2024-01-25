using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Model;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly AppDbContext _context;
        private ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrderCreatedEventConsumer(AppDbContext context, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();
            foreach(var item in context.Message.OrderItems)
            {
                stockResult.Add(await _context.Stock.AnyAsync(x => x.ProductId == item.ProductId && x.Count >= item.Count));
            }
            if(stockResult.Any(x => x == false))
            {
                _logger.LogInformation($"Stock is not reserved for Buyer Id:{context.Message.BuyerId}");
                await _publishEndpoint.Publish<StockNotReservedEvent>(new
                {
                    context.Message.OrderId,
                    Message = "Stock not reserved"
                });
            }
            else
            {
                foreach(var item in context.Message.OrderItems)
                {
                    var stock = await _context.Stock.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if(stock != null)
                        stock.Count -= item.Count;
                    _context.Stock.Update(stock);
                    await _context.SaveChangesAsync();              
                }

                _logger.LogInformation($"Stock is reserved for Buyer Id:{context.Message.BuyerId}");
                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.StockReservedEventQueueName}"));
                StockReservedEvent stockReservedEvent = new StockReservedEvent
                {
                    Payment = context.Message.Payment,
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    OrderItems = context.Message.OrderItems
                };
                await sendEndPoint.Send(stockReservedEvent);
            }
        }
    }
}
