FROM mcr.microsoft.com/dotnet/core/sdk:2.2.104 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY projectfiles.tar .
RUN tar -xvf projectfiles.tar && dotnet restore

COPY ./src ./src

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists /var/cache/apt/archives

RUN dotnet build -c Release --no-restore

# Copy everything else and build
RUN dotnet publish ./src/Schedule/Schedule.csproj -c Release -o /app/out --no-restore

# Build runtime image
microsoft/dotnet:2.2.104-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out/ .

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists /var/cache/apt/archives

RUN mkdir /logs

ENTRYPOINT ["dotnet", "Schedule.dll"]