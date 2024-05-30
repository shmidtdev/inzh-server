namespace IngServer.Dtos.Order;

public class OrderCommitDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public List<OrderCommitItem>? OrderCommitItems { get; set; }
}