using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Infrastructure.Security;

public class UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext db) : IUserAccessor
{
    public async Task<User> GetUserAsync()
    {
        return await db.Users.FindAsync(GetUserId()) ?? 
            throw new UnauthorizedAccessException("No user is logged in");
    }

    public string GetUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new Exception("No user found");
    }
}
