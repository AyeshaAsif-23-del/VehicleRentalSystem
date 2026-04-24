# Stage 1: Build the C# Application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files and build the app
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app/publish .

# to run application 
ENTRYPOINT ["dotnet", "VehicleRentalSystem.dll"]