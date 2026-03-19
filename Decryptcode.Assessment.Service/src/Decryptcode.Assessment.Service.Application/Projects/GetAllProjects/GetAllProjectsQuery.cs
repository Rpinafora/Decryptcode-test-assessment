using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Projects.GetAllProjects;

public sealed class GetAllProjectsQuery : IRequest<IEnumerable<ProjectDto>>
{
    public string? OrgId { get; set; }

    public string? Status { get; set; }
}
