using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Enums;
using Decryptcode.Assessment.Service.Domain.Guards;
using System.Diagnostics.CodeAnalysis;

namespace Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;

public sealed class Invoice : BaseEntity
{
    public string OrgId { get; private set; }

    public string ProjectId { get; private set; }

    public decimal Amount { get; private set; }

    // Altought this be a string, the best approach would be to have a dedicated entity for this,
    // with a predefined list of Currencies, but also allowing custom ones,
    // but for the sake of simplicity, we will keep it as a string.
    // Or dedicated endpoint that returns the list of supported currencies, and validate against it.
    public string Currency { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public DateTime? DueDate { get; private set; }

    public DateTime? IssuedAt { get; private set; }

    public string Description { get; private set; }

    public Organization? Organization { get; private set; }

    public Project? Project { get; private set; }

    [SetsRequiredMembers]
    private Invoice(
        string id,
        string orgId,
        string projectId,
        decimal amount,
        string currency,
        InvoiceStatus status,
        DateTime? dueDate,
        DateTime? issuedAt,
        string description)
    {
        Guard.NullOrWhiteSpace(id);
        Guard.NullOrWhiteSpace(orgId);
        Guard.NullOrWhiteSpace(projectId);
        Guard.OutOfRange(amount, 0, decimal.MaxValue);
        Guard.NullOrWhiteSpace(currency);
        Guard.NullOrWhiteSpace(description);

        Id = id;
        OrgId = orgId;
        ProjectId = projectId;
        Amount = amount;
        Currency = currency;
        Status = status;
        DueDate = dueDate;
        IssuedAt = issuedAt;
        Description = description;
    }

    public static Invoice Create(
        string id,
        string orgId,
        string projectId,
        decimal amount,
        string currency,
        InvoiceStatus status,
        DateTime? dueDate,
        DateTime? issuedAt,
        string description)
    {
        return new Invoice(id, orgId, projectId, amount, currency, status, dueDate, issuedAt, description);
    }

    public void ChangeStatusToSent()
    {
        Status = InvoiceStatus.Sent;
    }

    public void ChangeStatusToPaid()
    {
        Status = InvoiceStatus.Paid;
    }

    public void ChangeStatusToDraft()
    {
        Status = InvoiceStatus.Draft;
    }

    public void ChangeDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }

    public void ChangeIssuedAt(DateTime? issuedAt)
    {
        IssuedAt = issuedAt;
    }
    public void ChangeDescription(string description)
    {
        Guard.NullOrWhiteSpace(description);
        Description = description;
    }

    public void ChangeAmount(decimal amount)
    {
        Guard.OutOfRange(amount, 0, decimal.MaxValue);
        Amount = amount;
    }
}
