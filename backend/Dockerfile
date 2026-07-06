# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files
COPY Flira.slnx ./
COPY src/Flira.Domain/Flira.Domain.csproj src/Flira.Domain/
COPY src/Flira.Shared/Flira.Shared.csproj src/Flira.Shared/
COPY src/Flira.Application/Flira.Application.csproj src/Flira.Application/
COPY src/Flira.Infrastructure/Flira.Infrastructure.csproj src/Flira.Infrastructure/
COPY src/Flira.Persistence/Flira.Persistence.csproj src/Flira.Persistence/
COPY src/Flira.Api/Flira.Api.csproj src/Flira.Api/

# Restore packages
RUN dotnet restore Flira.slnx

# Copy source code and build
COPY src/ src/
RUN dotnet publish src/Flira.Api/Flira.Api.csproj -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "Flira.Api.dll"]
