#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MMOTFG_Bot/MMOTFG_Bot.csproj", "MMOTFG_Bot/"]
RUN dotnet restore "MMOTFG_Bot/MMOTFG_Bot.csproj"
COPY . .
WORKDIR "/src/MMOTFG_Bot"
RUN dotnet build "MMOTFG_Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MMOTFG_Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./MMOTFG_Bot/assets ./assets
ENTRYPOINT ["dotnet", "MMOTFG_Bot.dll"]