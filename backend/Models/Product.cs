using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Subtotal { get; set; }
        public int InvoiceId { get; set; }
    }
}
