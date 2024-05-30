using IngServer.DataBase;
using IngServer.DataBase.Models;
using IngServer.Dtos;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/wishLists/[action]")]
public class WishListController(
    ApplicationContext applicationContext,
    WishListRepository wishListRepository,
    ProductRepository productRepository,
    UserRepository userRepository) : Controller
{
    [HttpGet]
    public async Task<WishList?> GetWishList()
    {
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

        WishList? wishList = null;

        if (!isAuthenticated)
        {
            var wishListId = HttpContext.Request.Cookies["wishListId"];

            if (wishListId is null)
                return null;

            wishList = await wishListRepository.GetAsync(Guid.Parse(wishListId));
            if (wishList is null)
                return null;
        }
        else
        {
            var email = HttpContext.User.Identity?.Name;
            if (email is null)
                return null;

            var user = await userRepository.GetByEmailAsync(email);
            if (user is null)
                return null;

            wishList = await wishListRepository.GetByUserAsync(user) ?? await wishListRepository.CreateAsync(user);
        }

        return wishList;
    }
    
    [HttpPost]
    public async Task<WishList?> AddToWishList([FromBody] AddWishListPostDto dto)
    {
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

        var product = await productRepository.GetProductAsync(dto.ProductId);
        if (product is null)
            return null;

        WishList? wishList = null;
        
        if (!isAuthenticated)
        {
            var wishListId = Request.Cookies["wishListId"];

            if (wishListId is null)
            {
                wishList = await wishListRepository.CreateAsync(null);
                HttpContext.Response.Cookies.Append("wishListId", wishList.Id.ToString());

                var productMovement = new ProductMovement
                {
                    Id = Guid.NewGuid(),
                    Product = product,
                    CreationDate = DateTime.UtcNow
                };

                await applicationContext.ProductMovements.AddAsync(productMovement);
            }
            else
            {
                wishList = await wishListRepository.GetAsync(Guid.Parse(wishListId));

                if (wishList is null)
                {
                    wishList = await wishListRepository.CreateAsync(null);
                    HttpContext.Response.Cookies.Append("orderId", wishList.Id.ToString());
                }

                var productMovement = new ProductMovement
                {
                    Id = Guid.NewGuid(),
                    Product = product,
                    CreationDate = DateTime.UtcNow
                };

                await applicationContext.ProductMovements.AddAsync(productMovement);
                wishList.ProductMovements.Add(productMovement);
            }
        }
        else
        {
            var email = HttpContext.User.Identity?.Name;
            if (email is null)
                return null;

            var user = await userRepository.GetByEmailAsync(email);
            if (user is null)
                return null;

            wishList = await wishListRepository.GetByUserAsync(user) ?? await wishListRepository.CreateAsync(user);

            var productMovement = new ProductMovement
            {
                Id = Guid.NewGuid(),
                Product = product,
                CreationDate = DateTime.UtcNow
            };

            await applicationContext.ProductMovements.AddAsync(productMovement);
            wishList.ProductMovements.Add(productMovement);
        }

        await applicationContext.SaveChangesAsync();

        return wishList;
    }
    
    [HttpPost]
    public async Task<WishList?> RemoveFromWishList([FromBody] RemoveWishListPostDto dto)
    {
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

        var product = await productRepository.GetProductAsync(dto.ProductId);
        if (product is null)
            return null;

        WishList? wishList = null;
        
        if (!isAuthenticated)
        {
            var wishListId = Request.Cookies["wishListId"];

            if (wishListId is null)
                return null;

            wishList = await wishListRepository.GetAsync(Guid.Parse(wishListId));
            if (wishList is null)
                return null;

            await wishListRepository.RemoveProductMovementAsync(wishList.Id, dto.ProductId);
        }
        else
        {
            var email = HttpContext.User.Identity?.Name;
            if (email is null)
                return null;

            var user = await userRepository.GetByEmailAsync(email);
            if (user is null)
                return null;

            wishList = await wishListRepository.GetByUserAsync(user) ?? await wishListRepository.CreateAsync(user);
            
            await wishListRepository.RemoveProductMovementAsync(wishList.Id, dto.ProductId);
        }
        
        await applicationContext.SaveChangesAsync();

        return wishList;
    }
}