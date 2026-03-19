using Decryptcode.Assessment.Service.Application.TimeEntries.Dtos;
using Decryptcode.Assessment.Service.Application.TimeEntries.Mappings;
using Decryptcode.Assessment.Service.Application.Utils;
using Decryptcode.Assessment.Service.Application.Utils.Results;
using Decryptcode.Assessment.Service.Domain.Repositories;

namespace Decryptcode.Assessment.Service.Application.TimeEntries;

public sealed class GetAllTimeEntriesQueryHandler : IRequestHandler<GetAllTimeEntriesQuery, IEnumerable<TimeEntriesDto>>
{
    private readonly ITimeEntriesRepository _timeEntriesRepository;

    public GetAllTimeEntriesQueryHandler(ITimeEntriesRepository timeEntriesRepository)
    {
        _timeEntriesRepository = timeEntriesRepository;
    }

    public async Task<IRequestResult<IEnumerable<TimeEntriesDto>>> Handle(GetAllTimeEntriesQuery request, CancellationToken cancellationToken)
    {
        var entries = await _timeEntriesRepository.GetAllFiltered(
            request.UserId,
            request.ProjectId,
            request.From,
            request.To,
            TimeEntriesMappings.Projection,
            cancellationToken);

        return RequestResultFactory<IEnumerable<TimeEntriesDto>>.Ok(entries);
    }
}
