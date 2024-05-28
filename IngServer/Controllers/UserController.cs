using IngServer.Dtos;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/user/[action]")]
public class UserController(
    UserRepository userRepository
    ) : Controller
{
    [HttpGet]
    public async Task<UserContext?> GetUser()
    {
        var email = HttpContext.User.Identity?.Name;
        if (email is null)
            return null;

        var user = await userRepository.GetByEmailAsync(email);
        if (user is null)
            return null;
        
        return new UserContext
        {
            Name = user.Name ?? string.Empty,
            Email = user.Email,
            IsAuthorized = true,
            Phone = user?.Phone ?? string.Empty,
            UserRole = (int)user?.UserRole!
        };
    }
}