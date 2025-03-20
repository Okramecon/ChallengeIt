FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.sln .
COPY ["ChallengeIt/ChallengeIt.csproj", "ChallengeIt/"]
COPY ["ChallengeIt.API.Contracts/ChallengeIt.API.Contracts.csproj", "ChallengeIt.API.Contracts/"]
COPY ["ChallengeIt.Application/ChallengeIt.Application.csproj", "ChallengeIt.Application/"]
COPY ["ChallengeIt.Domain/ChallengeIt.Domain.csproj", "ChallengeIt.Domain/"]
COPY ["ChallengeIt.Infrastructure/ChallengeIt.Infrastructure.csproj", "ChallengeIt.Infrastructure/"]
RUN dotnet restore "./ChallengeIt/ChallengeIt.csproj"
COPY . .
WORKDIR "/src/ChallengeIt"
RUN dotnet build "./ChallengeIt.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChallengeIt.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChallengeIt.dll"]