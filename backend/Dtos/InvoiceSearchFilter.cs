namespace backend.Dtos
{
    public class InvoiceSearchFilter
    {
        public string? InvoiceNumber { get; set; }

        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }

    }
}
