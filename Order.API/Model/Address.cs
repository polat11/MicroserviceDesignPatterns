using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Model
{
    [Owned]
    public class Address
    {
        [MaxLength(50)]
        [Required]
        public string Line { get; set; }
        [MaxLength(50)]
        [Required]
        public string Province { get; set; }
        [MaxLength(50)]
        [Required]
        public string District { get; set; }
    }
}
