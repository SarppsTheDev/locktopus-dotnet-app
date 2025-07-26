using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace locktopus_domain.Helpers;

public class UserContextHelper(IHttpContextAccessor httpContextAccessor) : IUserContextHelper
{
    public string UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) 
                            ?? throw new UnauthorizedAccessException("User is not authenticated");

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}