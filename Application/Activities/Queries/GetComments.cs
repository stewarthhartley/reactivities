using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetComments
{
    public class Query : IRequest<Result<List<CommentDto>>>
    {
        public required string ActivityId { get; set; }        
    }

    public class Handler(AppDbContext db, IMapper mapper) : IRequestHandler<Query, Result<List<CommentDto>>>
    {
        public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var comments = await db.Comments.Where(c => c.ActivityId == request.ActivityId)
                .OrderByDescending(c => c.CreatedAt)
                .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<CommentDto>>.Success(comments);
        }
    }
}
