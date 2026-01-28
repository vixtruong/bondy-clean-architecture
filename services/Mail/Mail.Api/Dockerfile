# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Mail/Mail.Api/Mail.Api.csproj", "services/Mail/Mail.Api/"]
COPY ["shared/Bondy.Contracts/Bondy.Contracts.csproj", "shared/Bondy.Contracts/"]
COPY ["shared/Bondy.ServiceDefaults/Bondy.ServiceDefaults.csproj", "shared/Bondy.ServiceDefaults/"]
COPY ["shared/Bondy.SharedKernel.Api/Bondy.SharedKernel.Api.csproj", "shared/Bondy.SharedKernel.Api/"]
COPY ["shared/Bondy.SharedKernel.Application/Bondy.SharedKernel.Application.csproj", "shared/Bondy.SharedKernel.Application/"]
COPY ["shared/Bondy.SharedKernel.Domain/Bondy.SharedKernel.Domain.csproj", "shared/Bondy.SharedKernel.Domain/"]
COPY ["services/Mail/Mail.Application/Mail.Application.csproj", "services/Mail/Mail.Application/"]
COPY ["services/Mail/Mail.Domain/Mail.Domain.csproj", "services/Mail/Mail.Domain/"]
COPY ["services/Mail/Mail.Infrastructure/Mail.Infrastructure.csproj", "services/Mail/Mail.Infrastructure/"]
COPY ["shared/Bondy.SharedKernel.Infrastructure/Bondy.SharedKernel.Infrastructure.csproj", "shared/Bondy.SharedKernel.Infrastructure/"]
COPY ["services/Identity/Identity.Application/Identity.Application.csproj", "services/Identity/Identity.Application/"]
COPY ["shared/Storage/Shared.Storage.Abstractions/Shared.Storage.Abstractions.csproj", "shared/Storage/Shared.Storage.Abstractions/"]
COPY ["services/Identity/Identity.Domain/Identity.Domain.csproj", "services/Identity/Identity.Domain/"]
RUN dotnet restore "./services/Mail/Mail.Api/Mail.Api.csproj"
COPY . .
WORKDIR "/src/services/Mail/Mail.Api"
RUN dotnet build "./Mail.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mail.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY services/Mail/Mail.Infrastructure/Persistence/Migrations/Sql \
/app/migrations/sql

COPY services/Mail/Mail.Infrastructure/Templates \
/app/templates

ENTRYPOINT ["dotnet", "Mail.Api.dll"]