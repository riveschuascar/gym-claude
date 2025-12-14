<#
Arranca todos los proyectos .NET del monorepo en ventanas separadas.
Uso:  .\start-all.ps1 [-Configuration Release|Development]
#>
param(
    [ValidateSet("Development", "Release")]
    [string]$Configuration = "Development"
)

function Test-PortFree {
    param([Parameter(Mandatory)][int]$Port)
    $used = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    return -not $used
}

function Start-DotnetService {
    param(
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)][string]$ProjectPath,
        [Parameter(Mandatory)][string]$Url,
        [string]$LaunchProfile
    )

    $fullPath = Join-Path $PSScriptRoot $ProjectPath
    if (-not (Test-Path $fullPath)) {
        Write-Warning "No se encontr? $Name en $ProjectPath. Saltando."
        return
    }

    $uri = [Uri]$Url
    if (-not (Test-PortFree -Port $uri.Port)) {
        Write-Warning "$Name no se inicia: el puerto $($uri.Port) ya est? en uso. Cierra la instancia anterior o libera el puerto."
        return
    }

    $args = @("run", "--project", "`"$fullPath`"", "--configuration", $Configuration, "--urls", $Url)
    if ($LaunchProfile) { $args += @("--launch-profile", $LaunchProfile) }

    Write-Host "-> Iniciando $Name en $Url ..."
    $cmdArgs = '/k', 'dotnet', ($args -join " ")
    Start-Process -FilePath "cmd.exe" `
                  -ArgumentList $cmdArgs `
                  -WorkingDirectory (Split-Path $fullPath -Parent) `
                  -WindowStyle Normal
}
ReportMicroservice\ReportMicroservice.API\ReportMicroservice.API.csproj
$services = @(
    @{ Name = "Login API";            Path = "LoginMicroservice/LoginMicroservice/LoginMicroservice.csproj";       Url = "http://localhost:5289"; LaunchProfile = "http" },
    @{ Name = "User API";             Path = "UserMicroservice/UserMicroserviceApi/UserMicroserviceApi.csproj";    Url = "http://localhost:5089"; LaunchProfile = "http" },
    @{ Name = "Client API";           Path = "ClientMicroservice/ClientMicroservice/ClientMicroservice.csproj";    Url = "http://localhost:5135"; LaunchProfile = "http" },
    @{ Name = "Discipline API";       Path = "DisciplineMicroservice/DisciplineMicroserviceAPI/DisciplineMicroserviceApi.csproj"; Url = "http://localhost:5098"; LaunchProfile = "http" },
    @{ Name = "Membership API";       Path = "MembershipMicroservice/MembershipMicroserviceAPI/MembershipMicroserviceAPI.csproj"; Url = "http://localhost:5292"; LaunchProfile = "http" },
    @{ Name = "Sales API";            Path = "SalesMicroservice/SalesMicroserviceAPI/SalesMicroserviceAPI.csproj"; Url = "http://localhost:5305"; LaunchProfile = "http" },
    @{ Name = "Report API";           Path = "ReportMicroservice/ReportMicroservice.API/ReportMicroservice.API.csproj"; Url = "http://localhost:5236"; LaunchProfile = "http" },
    @{ Name = "Email gRPC";           Path = "EmailMicroservice/EmailMicroservice.API/EmailMicroservice.API.csproj"; Url = "http://localhost:5254"; LaunchProfile = "http" },
    @{ Name = "WebUI";                Path = "WebUI/WebUI/WebUI.csproj";                                            Url = "https://localhost:7241"; LaunchProfile = "https" }
)

foreach ($svc in $services) {
    Start-DotnetService -Name $svc.Name -ProjectPath $svc.Path -Url $svc.Url -LaunchProfile $svc.LaunchProfile
}

Write-Host "`nServicios lanzados (si el puerto estaba libre). Si alguno no arranc?, revisa el warning y libera el puerto." -ForegroundColor Green
