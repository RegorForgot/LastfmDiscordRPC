dotnet publish -r win-x86 -c Release --self-contained true -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
del "..\Release\LastfmDiscordRPC-SelfContained-x86.exe"
rename "..\Release\LastfmDiscordRPC.exe" "LastfmDiscordRPC-SelfContained-x86.exe"
dotnet publish -r win-x86 -c Release --self-contained false -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
del "..\Release\LastfmDiscordRPC-x86.exe"
rename "..\Release\LastfmDiscordRPC.exe" "LastfmDiscordRPC-x86.exe"
dotnet publish -r win-x64 -c Release --self-contained true -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
del "..\Release\LastfmDiscordRPC-SelfContained-x64.exe"
rename "..\Release\LastfmDiscordRPC.exe" "LastfmDiscordRPC-SelfContained-x64.exe"
dotnet publish -r win-x64 -c Release --self-contained false -o=..\Release /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishSingleFile=true
del "..\Release\LastfmDiscordRPC-x64.exe"
rename "..\Release\LastfmDiscordRPC.exe" "LastfmDiscordRPC-x64.exe"