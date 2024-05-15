namespace IngServer.Dtos;

public class OrderContextDto
{
    public List<OrderContextItem>? OrderContextItems { get; set; }
    public int TotalSum { get; set; }
}