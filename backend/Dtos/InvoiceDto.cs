namespace backend.Dtos
{
    public class InvoiceDto
    {
        public int Invoice_Number { get; set; }
        public DateTime Invoice_Date { get; set; }
        public string Invoice_Status { get; set; }
        public decimal Total_Amount { get; set; }
        public int Days_To_Due { get; set; }
        public DateTime Payment_Due_Date { get; set; }
        public string Payment_Status { get; set; }

        public List<InvoiceDetailDto> Invoice_Detail { get; set; }
        public InvoicePaymentDto Invoice_Payment { get; set; }
        public List<InvoiceCreditNoteDto> Invoice_Credit_Note { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
