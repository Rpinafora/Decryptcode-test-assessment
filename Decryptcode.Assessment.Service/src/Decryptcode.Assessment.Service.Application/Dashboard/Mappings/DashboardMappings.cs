using Decryptcode.Assessment.Service.Application.Dashboard.Dtos;
using Decryptcode.Assessment.Service.Domain.Entities.AggregateRoots;
using Decryptcode.Assessment.Service.Domain.Enums;
using System.Linq.Expressions;

namespace Decryptcode.Assessment.Service.Application.Dashboard.Mappings;

public static class DashboardMappings
{
    public static readonly Expression<Func<IQueryable<Organization>, IQueryable<DashboardDto>>> Projection = orgs =>
        orgs
            .Select(o => new DashboardDto
            {
                TotalOrganizations = orgs.Count(),
                TotalUsers = orgs.SelectMany(x => x.Users).Count(),
                TotalProjects = orgs.SelectMany(x => x.Projects).Count(),
                ActiveProjects = orgs.SelectMany(x => x.Projects).Count(p => p.Status == ProjectStatus.Active),
                TotalTimeEntries = orgs.SelectMany(x => x.Projects).SelectMany(p => p.TimeEntries).Count(),
                TotalInvoiced = orgs.SelectMany(x => x.Invoices).Sum(i => i.Amount)
            })
            .Take(1);
}
