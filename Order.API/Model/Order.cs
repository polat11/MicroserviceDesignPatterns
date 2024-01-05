using System.ComponentModel.DataAnnotations;

namespace Order.API.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(50)]
        [Required]
        public int BuyerId { get; set; }
        public Address Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public OrderStatus Status { get; set; }
        [MaxLength(300)]
        public string FailMessage { get; set; }
    }

    public enum OrderStatus
    {
        Suspend,
        Fail,
        Success
    }
}
