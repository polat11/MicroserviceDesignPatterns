using MassTransit;
using Shared;

namespace Payment.API.Consumer
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;
            if(balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"Payment is successful for order {context.Message.OrderId}");
                await _publishEndpoint.Publish(new PaymentSuccessEvent { OrderId = context.Message.OrderId, BuyerId = context.Message.BuyerId });
            }
            else
            {
                _logger.LogInformation($"Payment is failed for order {context.Message.OrderId}");
                await _publishEndpoint.Publish(new PaymentFailedEvent { OrderId = context.Message.OrderId, BuyerId = context.Message.BuyerId, Message="Balance is not enough", OrderItems = context.Message.OrderItems });
            }
        }
    }
}
