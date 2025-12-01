FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheBoard.API/TheBoard.API.csproj", "TheBoard.API/"]
COPY ["TheBoard.Application/TheBoard.Application.csproj", "TheBoard.Application/"]
COPY ["TheBoard.Infrastructure/TheBoard.Infrastructure.csproj", "TheBoard.Infrastructure/"]
COPY ["TheBoard.Domain/TheBoard.Domain.csproj", "TheBoard.Domain/"]
RUN dotnet restore "TheBoard.API/TheBoard.API.csproj"
COPY . .
WORKDIR "/src/TheBoard.API"
RUN dotnet build "./TheBoard.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TheBoard.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/db && chown -R $APP_UID:$APP_UID /app/db
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "TheBoard.API.dll"]
