using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Model;
using Shared;

namespace Order.API.Consumer
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        public StockNotReservedEventConsumer(AppDbContext context, ILogger<StockNotReservedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _context.Order.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
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
