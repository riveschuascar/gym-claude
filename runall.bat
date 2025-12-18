@echo off
title Lanzador de Microservicios
echo ===========================================
echo   Iniciando todos los microservicios...
echo ===========================================

REM ===========================
REM   Client Microservice (http://localhost:5135)
REM ===========================
start cmd /k "echo [Client] iniciando... & cd ClientMicroservice\ClientMicroservice && dotnet run --launch-profile http --urls http://localhost:5135"

REM ===========================
REM   Discipline Microservice (http://localhost:5098)
REM ===========================
start cmd /k "echo [Discipline] iniciando... & cd DisciplineMicroservice\DisciplineMicroserviceAPI && dotnet run --launch-profile http --urls http://localhost:5098"

REM ===========================
REM   Email Microservice gRPC (http://localhost:5254)
REM ===========================
start cmd /k "echo [Email] iniciando... & cd EmailMicroservice\EmailMicroservice.API && dotnet run --launch-profile http --urls http://localhost:5254"

REM ===========================
REM   Login Microservice (http://localhost:5289)
REM ===========================
start cmd /k "echo [Login] iniciando... & cd LoginMicroservice\LoginMicroservice && dotnet run --launch-profile http --urls http://localhost:5289"

REM ===========================
REM   Membership Microservice (http://localhost:5292)
REM ===========================
start cmd /k "echo [Membership] iniciando... & cd MembershipMicroservice\MembershipMicroserviceAPI && dotnet run --launch-profile http --urls http://localhost:5292"

REM ===========================
REM   Sales Microservice (http://localhost:5305)
REM ===========================
start cmd /k "echo [Sales] iniciando... & cd SalesMicroservice\SalesMicroserviceAPI && dotnet run --launch-profile http --urls http://localhost:5305"

REM ===========================
REM   User Microservice (http://localhost:5089)
REM ===========================
start cmd /k "echo [User] iniciando... & cd UserMicroservice\UserMicroserviceApi && dotnet run --launch-profile http --urls http://localhost:5089"

REM ===========================
REM   User Microservice (http://localhost:5089)
REM ===========================
start cmd /k "echo [Orchestator] iniciando... & cd OrchestratorMicroservice\Orchestrator.Rest && dotnet run --launch-profile http --urls http://localhost:5071"

REM ===========================
REM   Report Microservice (http://localhost:5236)
REM ===========================
start cmd /k "echo [Reportes] iniciando... & cd ReportMicroservice\ReportMicroservice.API && dotnet run --launch-profile http --urls http://localhost:5236"

REM ===========================
REM   SaleDetails Microservice (http://localhost:5079)
REM ===========================
start cmd /k "echo [SaleDetails] iniciando... & cd SaleDetailMicroservice\SaleDetailMicroservice && dotnet run --launch-profile http --urls http://localhost:5079"

REM ===========================
REM   WebUI (http://localhost:5030)
REM ===========================
start cmd /k "echo [WebUI] iniciando... & cd WebUI\WebUI && dotnet run --launch-profile http --urls http://localhost:5030"

echo.
echo Todos los microservicios estan iniciados.
echo.
pause
