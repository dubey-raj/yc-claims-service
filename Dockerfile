# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution file
COPY *.sln .

# Copy project files for the main project and class libraries
COPY src/ClaimService/*.csproj ./src/ClaimService/
COPY src/ClaimService.Services/*.csproj ./src/ClaimService.Services/
COPY src/ClaimService.Model/*.csproj ./src/ClaimService.Model/
COPY src/ClaimService.DataStorage/*.csproj ./src/ClaimService.DataStorage/

# Restore dependencies
RUN dotnet restore

# Copy the remaining files for all projects
COPY . .

# Build the project
WORKDIR /app/src/ClaimService
RUN dotnet publish -c Release -o /out

# Use the .NET runtime image for the final application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /out .

# Expose the application port (default for ASP.NET Core apps is 80)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "ClaimService.dll"]
