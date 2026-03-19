using Decryptcode.Assessment.Service.Application.Projects.Dtos;
using Decryptcode.Assessment.Service.Application.Projects.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.Projects.GetProjectById;

public sealed class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectSummaryDto>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IRequestResult<ProjectSummaryDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id, ProjectMappings.SummaryProjection, cancellationToken);

        if (project is null)
        {
            return RequestResultFactory<ProjectSummaryDto>.NotFound("Project not found");
        }

        return RequestResultFactory<ProjectSummaryDto>.Ok(project);
    }
}
