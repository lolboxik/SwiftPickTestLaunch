FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY SwiftPick.sln .
COPY SwiftPick.API/SwiftPick.API.csproj SwiftPick.API/
COPY SwiftPick.Core/SwiftPick.Core.csproj SwiftPick.Core/
COPY SwiftPick.Infrastructure/SwiftPick.Infrastructure.csproj SwiftPick.Infrastructure/
COPY SwiftPick.Services/SwiftPick.Services.csproj SwiftPick.Services/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/SwiftPick.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "SwiftPick.API.dll"]
