MSBuild.exe .\MMOTFG_Bot\MMOTFG_Bot\MMOTFG_Bot.csproj /t:ContainerBuild /p:Configuration=Release
docker image tag mmotfgbot telegrambotsregistry.azurecr.io/mmotfgbot:latest
docker push telegrambotsregistry.azurecr.io/mmotfgbot:latest