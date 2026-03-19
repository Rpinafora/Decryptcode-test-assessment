using Decryptcode.Assessment.Service.Application.Invoices.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Invoices.GetAllInvoices;

public sealed class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceDto>>
{
    public string? OrgId { get; set; }

    public string? Status { get; set; }
}
