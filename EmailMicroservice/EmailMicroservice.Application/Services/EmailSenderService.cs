using EmailMicroservice.Domain.Ports;

namespace EmailMicroservice.Application.Services;

public class EmailSenderService
{
    private readonly IEmailSender _emailSender;

    public EmailSenderService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Execute(string to, string subject, string body)
    {
        await _emailSender.SendAsync(to, subject, body);
    }
}
