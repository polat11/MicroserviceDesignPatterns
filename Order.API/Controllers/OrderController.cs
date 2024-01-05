using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Model;
using Order.API.DTOs;
using Shared;
using MassTransit;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrderController(AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO orderDTO)
        {
            var order = new Model.Order
            {
                BuyerId = orderDTO.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address
                {
                    Line = orderDTO.Address?.Line,
                    Province = orderDTO.Address?.Province,
                    District = orderDTO.Address?.District
                },
                CreatedDate = DateTime.Now,
            };
            orderDTO.OrderItems.ForEach(item =>
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Count = item.Count,
                });
            });
            order.FailMessage = "none";
            await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = order.BuyerId,
                OrderId = order.Id,
                Payment = new PaymentMessage
                {
                    CardNumber = orderDTO.Payment.CardNumber,
                    CardName = orderDTO.Payment.CardName,
                    Expiration = orderDTO.Payment.Expiration,
                    CVV = orderDTO.Payment.CVV,
                    TotalPrice = orderDTO.OrderItems.Sum(x => x.Price * x.Count)   
                },
            };
            orderDTO.OrderItems.ForEach(item =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage
                {
                    ProductId = item.ProductId,
                    Count = item.Count
                });
            });
            await _publishEndpoint.Publish(orderCreatedEvent);
            return Ok();
        }
    }
}
