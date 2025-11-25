<# 
Arranca todos los proyectos .NET del monorepo en ventanas separadas.
Uso:  .\start-all.ps1 [-Configuration Release|Development]
#>
param(
    [ValidateSet("Development", "Release")]
    [string]$Configuration = "Development"
)

function Start-DotnetService {
    param(
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)][string]$ProjectPath,
        [string]$LaunchProfile
    )

    $fullPath = Join-Path $PSScriptRoot $ProjectPath
    if (-not (Test-Path $fullPath)) {
        Write-Warning "No se encontró $Name en $ProjectPath. Saltando."
        return
    }

    $args = @("run", "--project", "`"$fullPath`"", "--configuration", $Configuration)
    if ($LaunchProfile) { $args += @("--launch-profile", $LaunchProfile) }

    Write-Host "-> Iniciando $Name ($ProjectPath) ..."
    # Abrimos cmd para que la ventana permanezca si el proceso termina (ver logs de arranque/errores)
    $cmdArgs = '/k', 'dotnet', ($args -join " ")
    Start-Process -FilePath "cmd.exe" `
                  -ArgumentList $cmdArgs `
                  -WorkingDirectory (Split-Path $fullPath -Parent) `
                  -WindowStyle Normal
}

$services = @(
    @{ Name = "Login API";            Path = "LoginMicroservice/LoginMicroservice/LoginMicroservice.csproj"; LaunchProfile = "http" },
    @{ Name = "User API";             Path = "UserMicroservice/UserMicroserviceApi/UserMicroserviceApi.csproj"; LaunchProfile = "http" },
    @{ Name = "Client API";           Path = "ClientMicroservice/ClientMicroservice/ClientMicroservice.csproj"; LaunchProfile = "http" },
    @{ Name = "Discipline API";       Path = "DisciplineMicroservice/DisciplineMicroserviceAPI/DisciplineMicroserviceApi.csproj"; LaunchProfile = "http" },
    @{ Name = "Membership API";       Path = "MembershipMicroservice/MembershipMicroserviceAPI/MembershipMicroserviceAPI.csproj"; LaunchProfile = "http" },
    @{ Name = "Email gRPC";           Path = "EmailMicroservice/EmailMicroservice.API/EmailMicroservice.API.csproj"; LaunchProfile = "http" },
    @{ Name = "WebUI";                Path = "WebUI/WebUI/WebUI.csproj"; LaunchProfile = "https" }
)

foreach ($svc in $services) {
    Start-DotnetService -Name $svc.Name -ProjectPath $svc.Path -LaunchProfile $svc.LaunchProfile
}

Write-Host "`nServicios lanzados (si la ruta existe). Verifica las ventanas para logs y puertos ocupados." -ForegroundColor Green
