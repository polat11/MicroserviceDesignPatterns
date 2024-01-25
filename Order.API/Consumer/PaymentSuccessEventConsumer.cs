using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Model;
using Shared;
using System.Diagnostics.Eventing.Reader;

namespace Order.API.Consumer
{
    public class PaymentSuccessEventConsumer : IConsumer<PaymentSuccessEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public PaymentSuccessEventConsumer(AppDbContext context, ILogger<PaymentSuccessEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
        {
            var order = await _context.Order.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
            if (order != null) {
                order.Status = OrderStatus.Success;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order {context.Message.OrderId} status changed to {OrderStatus.Success}");
            }
            else{
                _logger.LogError($"Order {context.Message.OrderId} not found");
            }
        }
    }
}
