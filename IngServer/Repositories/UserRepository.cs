using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class UserRepository(ApplicationContext applicationContext)
{
    public Task<User?> GetById(Guid id)
    {
        return applicationContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return applicationContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
    }

    public async Task CreateUserAsync(User user)
    {
        await applicationContext.Users.AddAsync(user);
    }

    // public async Task<User> Create()
    // {
    //     var user = new User
    //     {
    //         Id = Guid.NewGuid(),
    //         Email = null,
    //         Password = null,
    //         Name = null,
    //         Phone = null,
    //         UserRole = UserRole.Visitor,
    //         CreationDate = DateTime.UtcNow
    //     };
    //
    //     await applicationContext.Users.AddAsync(user);
    //     await applicationContext.SaveChangesAsync();
    // }
}