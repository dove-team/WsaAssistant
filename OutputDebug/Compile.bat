@echo off
cd "..\"
set Buildx64=%cd%\OutputDebug\Publish\x64
set BuildArm=%cd%\OutputDebug\Publish\arm64
set Libsx64=%cd%\OutputDebug\Publish\x64\Libs
set LibsArm64=%cd%\OutputDebug\Publish\arm64\Libs
if exist %Buildx64%\ (del /q/a/f/s %Buildx64%\*.*) 
if exist %BuildArm%\ (del /q/a/f/s %BuildArm%\*.*) 
dotnet publish -c Release -r win-x64 -o OutputDebug/Publish/x64 --self-contained true
dotnet publish -c Release -r win-arm64 -o OutputDebug/Publish/arm64 --self-contained true
xcopy %Libsx64% %Buildx64% /e /s /y /i
xcopy %LibsArm64% %BuildArm% /e /s /y /i
rd /q/s %Libsx64%
rd /q/s %LibsArm64%
pause