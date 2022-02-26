MSBuild.exe .\MMOTFG_Bot\MMOTFG_Bot.csproj /t:ContainerBuild /p:Configuration=Release
docker run -dt --rm mmotfgbot