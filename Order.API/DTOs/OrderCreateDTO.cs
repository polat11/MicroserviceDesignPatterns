namespace Order.API.DTOs
{
    public class OrderCreateDTO
    {
        public int BuyerId { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = [];
        public PaymentDTO? Payment { get; set; }
        public AddressDTO? Address { get; set; }
    }
}
