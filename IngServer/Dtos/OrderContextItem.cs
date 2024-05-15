using IngServer.DataBase.Models;

namespace IngServer.Dtos;

public class OrderContextItem
{
    public ProductMovement ProductMovement { get; set; }
    public int AmountOfElements { get; set; }
    public int Sum { get; set; }
}