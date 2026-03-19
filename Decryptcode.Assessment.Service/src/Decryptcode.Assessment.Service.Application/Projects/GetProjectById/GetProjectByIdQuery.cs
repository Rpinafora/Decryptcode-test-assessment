using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Utils;

namespace Decryptcode.Assessment.Service.Application.Projects.GetProjectById;

public sealed class GetProjectByIdQuery : IRequest<ProjectSummaryDto>
{
    public required string Id { get; set; }
}
