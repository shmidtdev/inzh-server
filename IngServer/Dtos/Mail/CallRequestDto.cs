namespace IngServer.Dtos.Call;

public class CallRequestDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public bool IsMailAllowed { get; set; }
}