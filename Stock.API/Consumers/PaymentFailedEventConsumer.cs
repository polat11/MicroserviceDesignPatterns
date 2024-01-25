using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Model;

namespace Stock.API.Consumers
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
            context.Message.OrderItems.ForEach(async x =>
            {
                var orderItem = await _context.Stock.FirstOrDefaultAsync(x => x.ProductId == x.ProductId);
                if (orderItem != null)
                {
                    orderItem.Count += x.Count;
                    _logger.LogInformation($"Stock count is updated for product {x.ProductId}");
                }
                else
                {
                    _logger.LogError($"Order item not found for product {x.ProductId}");
                }
                await _context.SaveChangesAsync();
            });

        }
    }
}
