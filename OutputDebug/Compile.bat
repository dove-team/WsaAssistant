@echo off
cd "..\"
set Buildx64="..\OutputDebug\Publish\x64"
set BuildArm="..\OutputDebug\Publish\arm64"
if exist %Buildx64%\ (del /q/a/f/s %Buildx64%\*.*) 
if exist %BuildArm%\ (del /q/a/f/s %BuildArm%\*.*) 
dotnet publish -c Release -r win-x64 -o OutputDebug/Publish/x64 --self-contained true
dotnet publish -c Release -r win-arm64 -o OutputDebug/Publish/arm64 --self-contained true
pause