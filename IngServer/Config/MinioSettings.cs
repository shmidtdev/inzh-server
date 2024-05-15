namespace IngServer.Config;

public class MinioSettings
{
    public string Host { get; set; }
    public string HostProtocol { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Buckeet { get; set; }
}