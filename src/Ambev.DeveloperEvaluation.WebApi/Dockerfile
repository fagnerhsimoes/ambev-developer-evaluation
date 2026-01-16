FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Install postgresql-client to use pg_isready (Robustness)
RUN apt-get update && apt-get install -y postgresql-client

# Copy all project files
COPY . .

# Restore dependencies for the WebApi project
RUN dotnet restore "src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj" -r linux-x64

# Publish the API directly, with fixed linux runtime for per/formance
RUN dotnet publish "src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj" \
    --configuration Release \
    --no-restore \
    -o /app/publish \
    -r linux-x64 \
    --self-contained false

# Final Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Security: Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Security: Switch to non-root user
USER appuser
ENTRYPOINT ["dotnet", "Ambev.DeveloperEvaluation.WebApi.dll"]