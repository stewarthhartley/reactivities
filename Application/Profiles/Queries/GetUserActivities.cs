using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetUserActivities
{
    public class Query : IRequest<Result<List<UserActivityDto>>>
    {
        public required string UserId { get; set; }
        public required string Filter { get; set; }
    }

    public class Handler(AppDbContext db, IMapper mapper) : IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
        public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = db.ActivityAttendees
                .Where(att => att.UserId == request.UserId)
                .OrderBy(att => att.Activity.Date)
                .Select(att => att.Activity)
                .AsQueryable();

            var today = DateTime.UtcNow;

            query = request.Filter switch
            {
                "past" => query.Where(a => a.Date <= today && a.Attendees.Any(att => att.UserId == request.UserId)),
                "hosting" => query.Where(a => a.Attendees.Any(att => att.IsHost && att.UserId == request.UserId)),
                _ => query.Where(a => a.Date >= today && a.Attendees.Any(att => att.UserId == request.UserId))
            };

            var activities = await query.ProjectTo<UserActivityDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<UserActivityDto>>.Success(activities);
        }
    }
}
