using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class InvoiceController : ControllerBase
    {
        private readonly SqliteDbContext _sqliteDbContext;

        public InvoiceController(SqliteDbContext sqliteDbContext)
        {
            _sqliteDbContext = sqliteDbContext;
        }

        [HttpGet("getAllInvoice")]
        public async Task<IActionResult> GetAllInvoice()
        {
            var invoices = await _sqliteDbContext.Invoices
                .Include(i => i.Products)
                .Include(i => i.CreditNotes)
                .ToListAsync();

            return Ok(invoices);
        }

        [HttpPost("creditNotesByInvoice")]
        public async Task<IActionResult> creditNotesByInvoice([FromBody] GetCreditNotesDto dto)
        {
            var creditNotes = await _sqliteDbContext.CreditNotes
                .Where(cn => cn.InvoiceId == dto.InvoiceId)
                .OrderBy(cn => cn.CreatedAt)
                .ToListAsync();

            return Ok(creditNotes);
        }


        [HttpPost("uploadInvoice")]
        public async Task<IActionResult> uploadInvoice(IFormFile fileInvoice)
        {
            List<int> rejected = new List<int>();

            List<Invoice> validInvoices = new List<Invoice>();
            try
            {
                if (fileInvoice == null || fileInvoice.Length == 0) return BadRequest("Invalid file.");

                using var stream = new StreamReader(fileInvoice.OpenReadStream());
                var jsonContent = await stream.ReadToEndAsync();

                var data = JsonSerializer.Deserialize<InvoiceWrapperDto>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null || data.Invoices == null || !data.Invoices.Any())
                    return BadRequest("JSON error.");

                var duplicatedInvoices = data.Invoices
                    .GroupBy(i => i.Invoice_Number)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .ToList();

                var uniqueInvoices = data.Invoices.GroupBy(i => i.Invoice_Number)
                    .Where(g => g.Count() == 1)
                    .SelectMany(g => g)
                    .ToList();


                foreach (var invoice in uniqueInvoices)
                {
                    if (_sqliteDbContext.Invoices.Any(i => i.InvoiceNumber == invoice.Invoice_Number.ToString()))
                    {
                        rejected.Add(invoice.Invoice_Number);
                        continue;
                    }


                    decimal sumSubtotals = invoice.Invoice_Detail.Sum(p => p.Subtotal);

                    if (sumSubtotals != invoice.Total_Amount)
                    {
                        rejected.Add(invoice.Invoice_Number);
                        continue;
                    }

                    decimal creditNotesTotal = invoice.Invoice_Credit_Note?.Sum(n => n.Credit_Note_Amount) ?? 0;

                    decimal outstandingBalance = invoice.Total_Amount - creditNotesTotal;

                    if (creditNotesTotal > outstandingBalance)
                    {
                        rejected.Add(invoice.Invoice_Number);
                        continue;
                    }


                    string invoiceStatus;

                    if (creditNotesTotal == invoice.Total_Amount)
                    {
                        invoiceStatus = "Cancelled";
                    }
                    else if (creditNotesTotal < invoice.Total_Amount)
                    {
                        invoiceStatus = "Partial";
                    }
                    else {
                        invoiceStatus = "Issued";
                    }


                    string paymentStatus;

                    if (invoice.Invoice_Payment?.Payment_Date != null)
                    {
                        paymentStatus = "Paid";
                    }
                    else if (DateTime.UtcNow.Date > invoice.Payment_Due_Date.Date)
                    {
                        paymentStatus = "Overdue";
                    }
                    else
                    {
                        paymentStatus = "Pending";
                    }

                    var createdInvoice = new Invoice
                    {
                        InvoiceNumber = invoice.Invoice_Number.ToString(),
                        IssueDate = invoice.Invoice_Date, //date
                        PaymentDueDate = invoice.Payment_Due_Date,
                        TotalAmount = invoice.Total_Amount,
                        Status = invoiceStatus,
                        PaymentStatus = paymentStatus,

                        CustomerRun = invoice.Customer.Customer_Run,
                        CustomerName = invoice.Customer.Customer_Name,
                        CustomerEmail = invoice.Customer.Customer_Email,

                        PaymentMethod = invoice.Invoice_Payment.Payment_Method,
                        PaymentDate = invoice.Invoice_Payment.Payment_Date,

                        Products = invoice.Invoice_Detail.Select(p => new Product
                        {
                            Description = p.Product_Name,
                            Subtotal = p.Subtotal,


                        }).ToList(),
                        CreditNotes = invoice.Invoice_Credit_Note?.Select(cn => new CreditNote
                        {
                            Amount = cn.Credit_Note_Amount,
                            CreatedAt = cn.Credit_Note_Date
                        }).ToList() ?? new List<CreditNote>()
                    };
                    
                    validInvoices.Add(createdInvoice);
                }

                  _sqliteDbContext.Invoices.AddRange(validInvoices);
                  await _sqliteDbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Process completed.",
                    loaded = validInvoices.Count,
                    rejected = rejected.Count,
                    duplicated = duplicatedInvoices.Count
                });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message
                });
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> InvoiceSearch([FromBody] InvoiceSearchFilter input)
        {
            try
            {
                var query = _sqliteDbContext.Invoices.Include(i => i.Products)
                                         .Include(i => i.CreditNotes)
                                         .AsQueryable();


                if (!string.IsNullOrEmpty(input.InvoiceNumber))
                    query = query.Where(i => i.InvoiceNumber == input.InvoiceNumber);


                if (!string.IsNullOrEmpty(input.PaymentStatus))
                    query = query.Where(i => i.PaymentStatus == input.PaymentStatus);

                if (!string.IsNullOrEmpty(input.Status))
                    query = query.Where(i => i.Status == input.Status);


                var result = await query.ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message
                });
            }
        }


        [HttpPost("creditNote")]
        public async Task<IActionResult> CreateCreditNote([FromBody] CreateCreditNoteDto input)
        {
            try
            {
                var invoice = await _sqliteDbContext.Invoices.Include(i => i.CreditNotes).FirstOrDefaultAsync(i => i.Id == input.invoiceId);

                if (invoice == null)
                    return BadRequest($"Invoice with ID {input.invoiceId} not found.");

                if (invoice.PaymentStatus == "Paid")
                    return BadRequest("You cannot add a credit note to an already paid invoice..");


                decimal creditNotesTotal = invoice.CreditNotes?.Sum(n => n.Amount) ?? 0;

                decimal outstandingBalance = invoice.TotalAmount - creditNotesTotal;

                if (input.Amount <= 0)
                    return BadRequest("The amount must be greater than 0.");

                if (input.Amount > outstandingBalance)
                    return BadRequest("The amount of the credit note exceeds the outstanding balance of the invoice.");

                var creditNote = new CreditNote
                {
                    Amount = input.Amount,
                    CreatedAt = DateTime.Now,
                    InvoiceId = invoice.Id
                };

                _sqliteDbContext.CreditNotes.Add(creditNote);
                _sqliteDbContext.SaveChanges();




                return Ok(new
                {
                    Message = "Credit note added successfully."
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message
                });
            }
        }



    }
}
