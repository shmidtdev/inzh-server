using System.Text;
using IngServer.DataBase;
using IngServer.DataBase.Models;
using IngServer.Dtos.Call;
using IngServer.Dtos.Mail;
using IngServer.Objects.MailExecutor;
using IngServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

[Route("api/mail/[action]")]
public class MailController(
    ApplicationContext applicationContext,
    SubscriberRepository subscriberRepository)
{
    [HttpPost]
    public async Task<bool> CallRequest([FromBody] CallRequestDto dto)
    {
        var message = new StringBuilder();
        message.Append($"<div>{dto.Email}</div>");
        message.Append($"<div>{dto.Phone}</div>");
        message.Append($"<div>{dto.Name}</div>");

        if (dto.IsMailAllowed)
            subscriberRepository.AddAsync(new Subscriber
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                SubscriptionDate = DateTime.UtcNow
            });

        await applicationContext.SaveChangesAsync();
        
        var mailExecutor = new MailExecutor();
        var isSent = await mailExecutor.SendAsync(message.ToString());

        return isSent;
    }

    [HttpPost]
    public async Task<bool> Subscribe([FromBody] SubscribeDto dto)
    {
        if (dto?.Email is null)
            return false;
        
        await subscriberRepository.AddAsync(new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            SubscriptionDate = DateTime.UtcNow
        });

        await applicationContext.SaveChangesAsync();

        return true;
    }
}