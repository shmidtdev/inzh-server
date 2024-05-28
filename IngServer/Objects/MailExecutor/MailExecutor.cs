using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace IngServer.Objects.MailExecutor;

public class MailExecutor
{
    private readonly string _address = "sale@ing-impgrp.ru";
    private readonly string _smtp = "smtp.ing-impgrp.ru";
    private readonly string _password = "uC8rK1cM4v";
    
    public async Task<bool> SendAsync(string message)
    {
        using var emailMessage = new MimeMessage();
        
        emailMessage.From.Add(new MailboxAddress("Инж Импорт Групп. Сайт", _address));
        emailMessage.To.Add(new MailboxAddress("", _address));
        emailMessage.Subject = "Заявка на продукцию";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        using var client = new SmtpClient();
        
        await client.ConnectAsync(_smtp, 587, SecureSocketOptions.None);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        await client.AuthenticateAsync(_address, _password, CancellationToken.None);
        await client.SendAsync(emailMessage, CancellationToken.None);

        return true;
    }
}