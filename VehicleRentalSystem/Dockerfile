# Build Stage - Using .NET 10.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project file and restore
COPY ["VehicleRentalSystem.csproj", "./"]
RUN dotnet restore "VehicleRentalSystem.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "VehicleRentalSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime:10.0-preview
WORKDIR /app
COPY --from=build /app/publish .

# Persistent Volume for SQLite Database
VOLUME /app/data

# Run the application
ENTRYPOINT ["dotnet", "VehicleRentalSystem.dll"]