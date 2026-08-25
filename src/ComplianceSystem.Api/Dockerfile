FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["ComplianceSystem.sln", "./"]

COPY ["src/ComplianceSystem.Api/ComplianceSystem.Api.csproj", "src/ComplianceSystem.Api/"]
COPY ["src/ComplianceSystem.Application/ComplianceSystem.Application.csproj", "src/ComplianceSystem.Application/"]
COPY ["src/ComplianceSystem.Domain/ComplianceSystem.Domain.csproj", "src/ComplianceSystem.Domain/"]
COPY ["src/ComplianceSystem.Infrastructure/ComplianceSystem.Infrastructure.csproj", "src/ComplianceSystem.Infrastructure/"]

RUN dotnet restore "src/ComplianceSystem.Api/ComplianceSystem.Api.csproj"

COPY . .

RUN dotnet publish "src/ComplianceSystem.Api/ComplianceSystem.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ComplianceSystem.Api.dll"]