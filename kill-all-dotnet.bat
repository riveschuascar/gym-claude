@echo off
REM Mata procesos dotnet y las consolas lanzadas por runall.bat
echo Cerrando procesos dotnet y consolas de microservicios...

REM Kill all dotnet hosts
powershell -NoProfile -Command "Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force"

REM Kill cmd.exe that were launched with dotnet run (runall abre cmd /k "dotnet run ...")
powershell -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='cmd.exe'\" | Where-Object { $_.CommandLine -match 'dotnet run' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }"

echo Listo. Si alguna ventana sigue abierta, ciérrala manualmente.
