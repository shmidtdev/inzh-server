using System.Security.Cryptography;
using System.Text;

namespace IngServer.Objects.User;

public class Encrypter
{
    public string Encrypt(string line)
    {
        using SHA256 sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(line));
        
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}