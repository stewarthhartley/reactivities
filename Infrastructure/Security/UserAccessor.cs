using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    public async Task<User> GetUserWithPhotosAsync()
    {
        var userId = GetUserId();

        return await db.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == userId) ?? 
            throw new UnauthorizedAccessException("No user is logged in");
    }
}
