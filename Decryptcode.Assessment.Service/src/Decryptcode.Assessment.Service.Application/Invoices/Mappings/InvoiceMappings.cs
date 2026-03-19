using Decryptcode.Assessment.Service.Application.Invoices.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.Invoices.Mappings;

public static class InvoiceMappings
{
    public static readonly Expression<Func<Invoice, InvoiceDto>> Projection = inv =>
        new InvoiceDto
        {
            Id = inv.Id,
            OrgId = inv.OrgId,
            ProjectId = inv.ProjectId,
            Amount = inv.Amount,
            Currency = inv.Currency,
            Status = inv.Status.ToString(),
            DueDate = inv.DueDate,
            IssuedAt = inv.IssuedAt,
            Description = inv.Description
        };
}
