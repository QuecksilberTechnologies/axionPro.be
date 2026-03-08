# ======================
# BUILD
# ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "AxionPro.sln"

RUN dotnet publish "axionpro.api/axionpro.api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ======================
# RUNTIME
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

# Render port binding
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "axionpro.api.dll"]