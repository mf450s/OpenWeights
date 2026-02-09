# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/Weights.API/Weights.API.csproj", "src/Weights.API/"]
COPY ["src/Weights.Application/Weights.Application.csproj", "src/Weights.Application/"]
COPY ["src/Weights.Domain/Weights.Domain.csproj", "src/Weights.Domain/"]
COPY ["src/Weights.Infrastructure/Weights.Infrastructure.csproj", "src/Weights.Infrastructure/"]

RUN dotnet restore "src/Weights.API/Weights.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Weights.API"
RUN dotnet build "Weights.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Weights.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

# Install ICU for localization support
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs icu-data-full

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weights.API.dll"]
