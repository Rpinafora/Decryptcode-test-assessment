using Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.ReferenceEntities;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.TimeEntries.Mappings;

public static class TimeEntriesMappings
{
    public static readonly Expression<Func<TimeEntry, TimeEntriesDto>> Projection = te =>
        new TimeEntriesDto
        {
            Id = te.Id,
            UserId = te.UserId,
            ProjectId = te.ProjectId,
            Date = te.Date,
            Hours = te.Hours,
            Description = te.Description
        };
}
