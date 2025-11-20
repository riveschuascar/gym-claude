using Grpc.Core;
using EmailMicroservice.Application.Services;
using EmailMicroservice.API;

namespace EmailMicroservice.API.Services;

public class EmailGrpcService : EmailService.EmailServiceBase
{
    private readonly EmailSenderService _useCase;

    public EmailGrpcService(EmailSenderService useCase)
    {
        _useCase = useCase;
    }

    public override async Task<SendCredentialEmailResponse> SendCredentialEmail(SendCredentialEmailRequest request, ServerCallContext context)
    {
        var subject = $"Credenciales - {request.Reason}";

        var body = $"Hola {request.UserFullName},\n\n" +
                   $"Se te envían tus credenciales por el siguiente motivo: {request.Reason}\n\n" +
                   $"Usuario: {request.Username}\n" +
                   $"Contraseña: {request.Password}\n\n" +
                   $"Recomendaciones de seguridad:\n{request.SecurityRecommendations}\n\n" +
                   "Saludos.";

        await _useCase.Execute(request.Email, subject, body);

        return new SendCredentialEmailResponse
        {
            Success = true,
            Message = "Correo enviado exitosamente"
        };
    }
}
