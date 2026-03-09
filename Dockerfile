# ======================
# BUILD STAGE
# ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# copy solution
COPY AxionPro.sln .

# copy projects
COPY axionpro.api/ axionpro.api/
COPY axionpro.application/ axionpro.application/
COPY axionpro.infrastructure/ axionpro.infrastructure/
COPY axionpro.persistance/ axionpro.persistance/
COPY axionpro.domain/ axionpro.domain/

# restore dependencies
RUN dotnet restore

# publish api
RUN dotnet publish axionpro.api/axionpro.api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ======================
# RUNTIME STAGE
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Supabase kerberos dependency fix
RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "axionpro.api.dll"]