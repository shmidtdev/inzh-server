using IngServer.DataBase;
using IngServer.DataBase.Models;

namespace IngServer.Repositories;

public class SubscriberRepository(ApplicationContext applicationContext)
{
    public async Task<bool> AddAsync(Subscriber subscriber)
    {
        await applicationContext.Subscribers.AddAsync(subscriber);

        return true;
    }
}