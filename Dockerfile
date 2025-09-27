# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the solution file and the project file(s)
COPY ["DotNetToDoWebApp/DotNetToDoWebApp.csproj", "DotNetToDoWebApp/"]

# Restore the dependencies specified in the project file
RUN dotnet restore "DotNetToDoWebApp/DotNetToDoWebApp.csproj"

# Copy the rest of the application code
COPY . .

# Publish the application to the /app/publish directory
RUN dotnet publish "DotNetToDoWebApp/DotNetToDoWebApp.csproj" -c Release -o /app/publish

# Use the official .NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Define the entry point for the application
ENTRYPOINT ["dotnet", "DotNetToDoWebApp.dll"]
