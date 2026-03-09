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

# install kerberos dependency (fix Supabase Npgsql error)
RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

# copy published app
COPY --from=build /app/publish .

# Render dynamic port binding
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "axionpro.api.dll"]