FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY projectfiles.tar .
RUN tar -xvf projectfiles.tar && dotnet restore

COPY ./test ./test
COPY ./src ./src

RUN dotnet build -c Release --no-restore

# Copy everything else and build
RUN dotnet publish ./src/Schedule/Schedule.csproj -c Release -o /app/out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build-env /app/out/ .

RUN mkdir /logs

ENTRYPOINT ["dotnet", "Schedule.dll"]