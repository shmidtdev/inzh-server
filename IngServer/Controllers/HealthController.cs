using Microsoft.AspNetCore.Mvc;

namespace IngServer.Controllers;

public class HealthController
{
    [Route("api/health")]
    [HttpGet]
    public string Health()
    {
        return "Healthy";
    }
}