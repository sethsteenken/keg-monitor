FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

RUN apt-get update \
    && apt-get install -y curl

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/KegMonitor.Web/KegMonitor.Web.csproj", "src/KegMonitor.Web/"]
RUN dotnet restore "src/KegMonitor.Web/KegMonitor.Web.csproj"
COPY . .
WORKDIR "/src/src/KegMonitor.Web"
RUN dotnet build "KegMonitor.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KegMonitor.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KegMonitor.Web.dll"]
