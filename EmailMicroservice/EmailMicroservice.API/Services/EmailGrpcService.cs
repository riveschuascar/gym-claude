using Grpc.Core;
using EmailMicroservice.Application.Services;

namespace EmailMicroservice.API.Services;

public class EmailGrpcService : EmailService.EmailServiceBase
{
    private readonly EmailSenderService _useCase;

    public EmailGrpcService(EmailSenderService useCase)
    {
        _useCase = useCase;
    }

    public override async Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
    {
        await _useCase.Execute(request.To, request.Subject, request.Body);

        return new SendEmailResponse
        {
            Success = true,
            Message = "Correo enviado exitosamente"
        };
    }
}
