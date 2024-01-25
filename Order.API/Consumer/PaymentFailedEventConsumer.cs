using MassTransit;
using Order.API.Model;
using Shared;

namespace Order.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public PaymentFailedEventConsumer(AppDbContext context, ILogger<PaymentFailedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = _context.Order.FirstOrDefault(x => x.Id == context.Message.OrderId);
            if(order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order status changed to {order.Status}");
            }
            else
            {
                _logger.LogError($"Order not found with id:{context.Message.OrderId}");
            }
        }
    }
}
