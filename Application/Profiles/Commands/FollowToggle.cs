using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string TargetUserId { get; set; }
    }

    public class Handler(AppDbContext db, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var observer = await userAccessor.GetUserAsync();
            var target = await db.Users.FindAsync(request.TargetUserId, cancellationToken);

            if (target == null) return Result<Unit>.Failure("Target user not found", 400);

            var following = await db.UserFollowings.FindAsync([observer.Id, target.Id], cancellationToken);

            if (following == null)
            {
                db.UserFollowings.Add(new UserFollowing() { ObserverId = observer.Id, TargetId = target.Id });
            }
            else
            {
                db.UserFollowings.Remove(following);   
            }

            return await db.SaveChangesAsync(cancellationToken) > 0
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem updating following", 400);
        }
    }
}
