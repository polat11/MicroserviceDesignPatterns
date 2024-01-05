using System.ComponentModel.DataAnnotations;

namespace Order.API.DTOs
{
    public class AddressDTO
    {

        public string? Line { get; set; }
 
        public string? Province { get; set; }
  
        public string? District { get; set; }
    }
}
