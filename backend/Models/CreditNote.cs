using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class CreditNote
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InvoiceId { get; set; }
    }
}
