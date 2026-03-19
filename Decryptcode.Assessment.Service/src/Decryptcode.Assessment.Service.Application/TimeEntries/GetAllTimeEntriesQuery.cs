using Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.TimeEntries;

public sealed class GetAllTimeEntriesQuery : IRequest<IEnumerable<TimeEntriesDto>>
{
    public string? UserId { get; set; }

    public string? ProjectId { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}
