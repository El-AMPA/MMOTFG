#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PruebaSQL/PruebaSQL.csproj", "PruebaSQL/"]
RUN dotnet restore "PruebaSQL/PruebaSQL.csproj"
COPY . .
WORKDIR "/src/PruebaSQL"
RUN dotnet build "PruebaSQL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PruebaSQL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PruebaSQL.dll"]