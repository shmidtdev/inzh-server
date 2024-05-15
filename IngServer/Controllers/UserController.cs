using IngServer.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/user/[action]")]
public class UserController
{
    [HttpPost]
    public bool CreateUser()
    {
        return true;
    }

    [HttpGet]
    public User GetUser()
    {
        return new User();
    }
}