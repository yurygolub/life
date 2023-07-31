@echo off

where /q dotnet
if %ERRORLEVEL% NEQ 0 goto missingDotnet

dotnet publish Life --configuration Release --output publish/Life -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:SatelliteResourceLanguages=en --self-contained

dotnet publish GameOfLife --configuration Release --output publish/GameOfLife -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:SatelliteResourceLanguages=en --self-contained


goto exit

:missingDotnet
echo dotnet was not found.
echo You can download it here https://dotnet.microsoft.com/en-us/download/dotnet/7.0
goto exit

:exit
