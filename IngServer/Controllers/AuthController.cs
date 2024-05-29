using System.Security.Claims;
using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;
using IngServer.Dtos.Auth;
using IngServer.Objects.User;
using IngServer.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/auth/[action]")]
public class AuthController(
    ApplicationContext applicationContext,
    UserRepository userRepository,
    Encrypter encrypter
) : Controller
{
    [HttpPost]
    public async Task<bool> Registration([FromBody] RegistrationDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email);
        if (user is not null)
            return false;

        var encryptedPassword = encrypter.Encrypt(dto.Password);

        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = dto.Name,
            Phone = dto.Phone,
            Password = encryptedPassword,
            IsMailAllowed = dto.IsMailAllowed,
            UserRole = UserRole.Visitor,
            CreationDate = DateTime.UtcNow
        };

        await userRepository.CreateUserAsync(createdUser);
        await applicationContext.SaveChangesAsync();
        
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, createdUser.Email)
        };

        var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return true;
    }

    [HttpPost]
    public async Task<bool> SignIn([FromBody] SignInDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email);
        if (user is null)
            return false;
        
        var encryptedPassword = encrypter.Encrypt(dto.Password);

        if (encryptedPassword != user.Password)
            return false;
        
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, user.Email)
        };

        var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return true;
    }
    
    [HttpGet]
    public new async Task<bool> SignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return true;
    }
}