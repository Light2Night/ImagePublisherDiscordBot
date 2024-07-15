# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# copy csproj and restore as distinct layers
WORKDIR /src
COPY ImagePublisherDiscordBot.sln ./
COPY Bot/*.csproj ./Bot/
RUN dotnet restore

# copy everything else and build app
COPY . .

WORKDIR /src/Bot
RUN dotnet publish -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Bot.dll"]