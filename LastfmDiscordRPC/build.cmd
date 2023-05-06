dotnet publish -r win-x64 -c Release --self-contained false -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
iscc ".\Installer\Installer.iss"
