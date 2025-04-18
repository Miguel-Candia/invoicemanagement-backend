namespace backend.Dtos
{
    public class InvoiceDetailDto
    {

        public string Product_Name { get; set; }
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }

    }
}
