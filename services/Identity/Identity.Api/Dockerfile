# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Identity/Identity.Api/Identity.Api.csproj", "services/Identity/Identity.Api/"]
COPY ["shared/Bondy.ServiceDefaults/Bondy.ServiceDefaults.csproj", "shared/Bondy.ServiceDefaults/"]
COPY ["shared/Bondy.SharedKernel.Api/Bondy.SharedKernel.Api.csproj", "shared/Bondy.SharedKernel.Api/"]
COPY ["shared/Bondy.SharedKernel.Application/Bondy.SharedKernel.Application.csproj", "shared/Bondy.SharedKernel.Application/"]
COPY ["shared/Bondy.SharedKernel.Domain/Bondy.SharedKernel.Domain.csproj", "shared/Bondy.SharedKernel.Domain/"]
COPY ["services/Identity/Identity.Application/Identity.Application.csproj", "services/Identity/Identity.Application/"]
COPY ["shared/Storage/Shared.Storage.Abstractions/Shared.Storage.Abstractions.csproj", "shared/Storage/Shared.Storage.Abstractions/"]
COPY ["services/Identity/Identity.Domain/Identity.Domain.csproj", "services/Identity/Identity.Domain/"]
COPY ["services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "services/Identity/Identity.Infrastructure/"]
COPY ["shared/Bondy.SharedKernel.Infrastructure/Bondy.SharedKernel.Infrastructure.csproj", "shared/Bondy.SharedKernel.Infrastructure/"]
COPY ["shared/Storage/Shared.Storage.Local/Shared.Storage.Local.csproj", "shared/Storage/Shared.Storage.Local/"]
RUN dotnet restore "./services/Identity/Identity.Api/Identity.Api.csproj"
COPY . .
WORKDIR "/src/services/Identity/Identity.Api"
RUN dotnet build "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]