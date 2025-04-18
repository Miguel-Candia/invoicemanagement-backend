using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string CustomerRun { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public List<Product> Products { get; set; }
        public List<CreditNote> CreditNotes { get; set; }
    }
}
