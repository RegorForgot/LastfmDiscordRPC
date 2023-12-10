dotnet publish -r linux-x64 --self-contained false -c Release -p:IncludeAllContentForSelfExtract=true -p:UseAppHost=true
dotnet publish -r osx-arm64 --self-contained false -c Release -p:IncludeAllContentForSelfExtract=true -p:UseAppHost=true
dotnet publish -r osx-x64 --self-contained false -c Release -p:IncludeAllContentForSelfExtract=true -p:UseAppHost=true
dotnet publish -r win8-x64 --self-contained false -c Release -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

cd LastfmDiscordRPC2\bin\Release\net7.0
del linux-x64.tar.gz
del osx-arm64.tar.gz
del osx-x64.tar.gz
del win8-x64.zip

cd linux-x64
7z.exe a -ttar ..\linux-x64.tar publish\*
7z.exe rn ..\linux-x64.tar publish LastfmDiscordRPC2
7z.exe a -tgzip -sdel ..\linux-x64.tar.gz ..\linux-x64.tar

cd ..\osx-arm64
7z.exe a -ttar ..\osx-arm64.tar publish\*
7z.exe rn ..\osx-arm64.tar publish LastfmDiscordRPC2\bin
7z.exe a -ttar ..\osx-arm64.tar ..\run.sh
7z.exe rn ..\osx-arm64.tar run.sh LastfmDiscordRPC2\run.sh
7z.exe a -tgzip -sdel ..\osx-arm64.tar.gz ..\osx-arm64.tar

cd ..\osx-x64
7z.exe a -ttar ..\osx-x64.tar publish\*
7z.exe rn ..\osx-x64.tar publish LastfmDiscordRPC2
7z.exe a -tgzip -sdel ..\osx-x64.tar.gz ..\osx-x64.tar

cd ..\win8-x64
7z.exe a -tzip ..\win8-x64.zip .\publish\LastfmDiscordRPC2.exe

pause