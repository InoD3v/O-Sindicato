# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Syndicate.slnx .
COPY Syndicate.API/Syndicate.API.csproj Syndicate.API/
COPY Syndicate.Domain/Syndicate.Domain.csproj Syndicate.Domain/
COPY Syndicate.Infrastructure/Syndicate.Infrastructure.csproj Syndicate.Infrastructure/
RUN dotnet restore

COPY . .
RUN dotnet publish Syndicate.API/Syndicate.API.csproj -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Syndicate.API.dll"]