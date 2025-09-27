# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire repository into the container
COPY . .

# Restore the project's dependencies
RUN dotnet restore DotNetToDoWebApp/DotNetToDoWebApp.csproj

# Publish the application to the /app/publish directory
RUN dotnet publish DotNetToDoWebApp/DotNetToDoWebApp.csproj -c Release -o /app/publish

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port 5000 for the application
EXPOSE 5000

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "DotNetToDoWebApp.dll"]
