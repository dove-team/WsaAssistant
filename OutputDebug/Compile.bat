@echo off
cd "..\"
set Buildx64="OutputDebug\Publish\x64"
set BuildArm="OutputDebug\Publish\arm64"
set Libsx64="OutputDebug\Publish\x64\Libs"
set LibsArm64="OutputDebug\Publish\arm64\Libs"
if exist %Buildx64%\ (del /q/a/f/s %Buildx64%\*.*) 
if exist %BuildArm%\ (del /q/a/f/s %BuildArm%\*.*) 
dotnet publish -c Release -r win-x64 -o OutputDebug/Publish/x64 --self-contained true
dotnet publish -c Release -r win-arm64 -o OutputDebug/Publish/arm64 --self-contained true
xcopy %Libsx64% %Buildx64% /q /e /r /s /y
xcopy %LibsArm64% %BuildArm% /q /e /r /s /y
pause