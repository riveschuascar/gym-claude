@echo off
title Lanzador de Microservicios
echo ===========================================
echo   Iniciando todos los microservicios...
echo ===========================================

REM ===========================
REM   Client Microservice
REM ===========================
start cmd /k "echo [Client] iniciando... & cd ClientMicroservice\ClientMicroservice && dotnet run"

REM ===========================
REM   Discipline Microservice
REM ===========================
start cmd /k "echo [Discipline] iniciando... & cd DisciplineMicroservice\DisciplineMicroserviceAPI && dotnet run"

REM ===========================
REM   Email Microservice
REM ===========================
start cmd /k "echo [Email] iniciando... & cd EmailMicroservice\EmailMicroservice.API && dotnet run"

REM ===========================
REM   Login Microservice
REM ===========================
start cmd /k "echo [Login] iniciando... & cd LoginMicroservice\LoginMicroservice && dotnet run"

REM ===========================
REM   Membership Microservice
REM ===========================
start cmd /k "echo [Membership] iniciando... & cd MembershipMicroservice\MembershipMicroserviceAPI && dotnet run"

REM ===========================
REM   User Microservice
REM ===========================
start cmd /k "echo [User] iniciando... & cd UserMicroservice\UserMicroserviceApi && dotnet run"

REM ===========================
REM   WebUI
REM ===========================
start cmd /k "echo [WebUI] iniciando... & cd WebUI\WebUI && dotnet run"

echo.
echo Todos los microservicios estan iniciados.
echo.
pause
