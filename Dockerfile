# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy everything
COPY . .

# Restore all projects
RUN dotnet restore "AxionPro.sln"

# Build and publish
RUN dotnet publish "axionpro.api/axionpro.api.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "axionpro.api.dll"]
