using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfilePhotos
{
    public class Query : IRequest<Result<List<Photo>>>
    {
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext db) : IRequestHandler<Query, Result<List<Photo>>>
    {
        public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var photos = await db.Users.Where(u => u.Id == request.UserId)
                .SelectMany(u => u.Photos).ToListAsync(cancellationToken);

            return Result<List<Photo>>.Success(photos);
        }
    }
}
