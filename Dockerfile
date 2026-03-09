# ======================
# BUILD STAGE
# ======================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# copy entire repository
COPY . .

# restore solution
RUN dotnet restore AxionPro.sln

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

# Fix Supabase Npgsql kerberos dependency
RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

# copy build output
COPY --from=build /app/publish .

# Render port binding
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "axionpro.api.dll"]