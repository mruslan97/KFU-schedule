FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Schedule/Schedule.csproj", "Schedule/"]
COPY ["Storage.Abstractions/Storage.Abstractions.csproj", "Storage.Abstractions/"]
COPY ["Storage.EFCore/Storage.EFCore.csproj", "Storage.EFCore/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Schedule.Entities/Schedule.Entities.csproj", "Schedule.Entities/"]
COPY ["Vk.Bot.Framework/Vk.Bot.Framework.csproj", "Vk.Bot.Framework/"]
COPY ["Storage.Migrations/Storage.Migrations.csproj", "Storage.Migrations/"]
RUN dotnet restore "Schedule/Schedule.csproj"
COPY . .
WORKDIR "/src/Schedule"
RUN dotnet build "Schedule.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Schedule.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Schedule.dll"]