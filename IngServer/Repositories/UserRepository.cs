using IngServer.DataBase;
using IngServer.DataBase.Enums;
using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.Repositories;

public class UserRepository(ApplicationContext applicationContext)
{
    public async Task<User?> GetById(Guid id)
    {
        return await applicationContext.Users.FirstOrDefaultAsync(x => x.Id == id);
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