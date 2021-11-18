@echo off
set Buildx64="..\Publish\x64"
set BuildArm="..\Publish\arm64"
if exist %Buildx64%\ (del /q/a/f/s %Buildx64%\*.*) 
if exist %BuildArm%\ (del /q/a/f/s %BuildArm%\*.*) 
dotnet publish -c Release -r win-x64 -o Publish/x64 --self-contained true
dotnet publish -c Release -r win-arm64 -o Publish/arm64 --self-contained true
pause