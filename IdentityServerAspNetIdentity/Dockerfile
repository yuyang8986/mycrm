FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["IdentityServerAspNetIdentity/IdentityServerAspNetIdentity.csproj", "IdentityServerAspNetIdentity/"]
COPY ["MyCRM.Shared/MyCRM.Shared.csproj", "MyCRM.Shared/"]
COPY ["MyCRM.Persistence/MyCRM.Persistence.csproj", "MyCRM.Persistence/"]
RUN dotnet restore "IdentityServerAspNetIdentity/IdentityServerAspNetIdentity.csproj"
COPY . .
WORKDIR "/src/IdentityServerAspNetIdentity"
RUN dotnet build "IdentityServerAspNetIdentity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IdentityServerAspNetIdentity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServerAspNetIdentity.dll"]