FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["MyCRM.API/MyCRM.API.csproj", "MyCRM.API/"]
COPY ["MyCRM.Shared/MyCRM.Shared.csproj", "MyCRM.Shared/"]
COPY ["MyCRM.Services/MyCRM.Services.csproj", "MyCRM.Services/"]
COPY ["IdentityServerAspNetIdentity/IdentityServerAspNetIdentity.csproj", "IdentityServerAspNetIdentity/"]
COPY ["MyCRM.Persistence/MyCRM.Persistence.csproj", "MyCRM.Persistence/"]
RUN dotnet restore "MyCRM.API/MyCRM.API.csproj"
COPY . .
WORKDIR "/src/MyCRM.API"
RUN dotnet build "MyCRM.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MyCRM.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyCRM.API.dll"]