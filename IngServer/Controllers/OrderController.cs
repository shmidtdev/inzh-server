using IngServer.DataBase;
using IngServer.DataBase.Models;
using IngServer.Dtos;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/order/[action]")]
public class OrderController(
    OrderRepository orderRepository,
    ProductRepository productRepository,
    ApplicationContext applicationContext) : Controller
{
    [HttpPost]
    public async Task<OrderContextDto?> AddToOrder([FromBody] AddOrderPostDto dto)
    {
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

        var product = await productRepository.GetProductAsync(dto.ProductId);
        if (product is null)
            return null;

        Order? order = null;
        
        if (!isAuthenticated)
        {
            var orderId = Request.Cookies["orderId"];

            if (orderId is null)
            {
                order = await orderRepository.CreateAsync();
                HttpContext.Response.Cookies.Append("orderId", order.Id.ToString());

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
                order = await orderRepository.GetAsync(Guid.Parse(orderId));

                if (order is null)
                {
                    order = await orderRepository.CreateAsync();
                    HttpContext.Response.Cookies.Append("orderId", order.Id.ToString());
                }

                var productMovement = new ProductMovement
                {
                    Id = Guid.NewGuid(),
                    Product = product,
                    CreationDate = DateTime.UtcNow
                };

                await applicationContext.ProductMovements.AddAsync(productMovement);
                order.ProductMovements.Add(productMovement);
            }
        }

        await applicationContext.SaveChangesAsync();

        return GetOrderContextDto(order);
    }

    [HttpPost]
    public async Task<OrderContextDto?> RemoveFromOrder([FromBody] RemoveOrderPostDto dto)
    {
        var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;

        var product = await productRepository.GetProductAsync(dto.ProductId);
        if (product is null)
            return null;

        Order? order = null;
        
        if (!isAuthenticated)
        {
            var orderId = Request.Cookies["orderId"];

            if (orderId is null)
                return null;

            order = await orderRepository.GetAsync(Guid.Parse(orderId));
            if (order is null)
                return null;

            await orderRepository.RemoveProductMovementAsync(order.Id, dto.ProductId);

            await applicationContext.SaveChangesAsync();
        }

        return GetOrderContextDto(order);
    }

    [HttpGet]
    public async Task<OrderContextDto?> GetOrder()
    {
        var orderId = HttpContext.Request.Cookies["orderId"];

        if (orderId is null)
            return null;

        var order = await orderRepository.GetAsync(Guid.Parse(orderId));
        if (order is null)
            return null;

        return GetOrderContextDto(order);
    }

    private OrderContextDto? GetOrderContextDto(Order order)
    {
        var orderContextItems = order.ProductMovements.GroupBy(x => x.Product).Select(x => new OrderContextItem
        {
            ProductMovement = x.First(),
            AmountOfElements = x.Count(),
            Sum = (int)(x.Count() * x.Key.Price)
        }).OrderBy(x => x.ProductMovement.Product.TitleEng).ToList();
        
        return new OrderContextDto
        {
            OrderContextItems = orderContextItems,
            TotalSum = orderContextItems.Sum(x => x.Sum)
        };
    }
}