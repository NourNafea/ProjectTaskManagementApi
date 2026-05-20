using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
