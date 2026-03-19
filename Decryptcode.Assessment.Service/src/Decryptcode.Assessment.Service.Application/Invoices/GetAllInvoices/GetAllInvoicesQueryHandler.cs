using Decryptcode.Assessment.Service.Application.Invoices.Dtos;
using Decryptcode.Assessment.Service.Application.Invoices.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Invoices.GetAllInvoices;

public sealed class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IRequestResult<IEnumerable<InvoiceDto>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllFiltered(request.OrgId, request.Status, InvoiceMappings.Projection, cancellationToken);

        return RequestResultFactory<IEnumerable<InvoiceDto>>.Ok(invoices);
    }
}
