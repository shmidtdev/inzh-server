namespace IngServer.DataBase.Models;

public class Subscriber
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTime SubscriptionDate { get; set; }
}