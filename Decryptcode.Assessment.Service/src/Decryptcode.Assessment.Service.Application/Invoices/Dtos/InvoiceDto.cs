namespace Decryptcode.Assessment.Service.Application.Invoices.Dtos;

public sealed record InvoiceDto
{
    public required string Id { get; init; }

    public required string OrgId { get; init; }

    public required string ProjectId { get; init; }

    public decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Status { get; init; }

    public DateTime? DueDate { get; init; }

    public DateTime? IssuedAt { get; init; }

    public required string Description { get; init; }
}
