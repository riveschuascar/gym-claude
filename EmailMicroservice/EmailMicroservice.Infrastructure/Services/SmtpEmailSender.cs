using EmailMicroservice.Domain.Ports;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailMicroservice.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;

    public SmtpEmailSender(string host, int port, string username, string password)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Servicio", _username));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var client = new SmtpClient(new ProtocolLogger("smtp.log"));
            await client.ConnectAsync(_host, _port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (AuthenticationException ex)
        {
            throw new InvalidOperationException("SMTP authentication failed. Revisa usuario/contraseña o app password.", ex);
        }
        catch (SmtpProtocolException ex)
        {
            throw;
        }
    }
}
