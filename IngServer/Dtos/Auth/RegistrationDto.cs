namespace IngServer.Dtos.Auth;

public class RegistrationDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public bool IsMailAllowed { get; set; }
}