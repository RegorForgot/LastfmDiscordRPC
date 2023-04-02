dotnet publish -r win-x64 -c Release --self-contained false -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
del "..\Release\LastfmDiscordRPC-x64.exe"
rename "..\Release\LastfmDiscordRPC.exe" "LastfmDiscordRPC-x64.exe"

iscc ".\Installer\Non-SC-Installer.iss"
del "..\Release\LastfmDiscordRPC.exe"
rename "..\Release\LastfmDiscordRPCInstaller.exe" "LastfmDiscordRPC.exe"
