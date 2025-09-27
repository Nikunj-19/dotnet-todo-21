FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore DotNetToDoWebApp/DotNetToDoWebApp.csproj
RUN dotnet publish DotNetToDoWebApp/DotNetToDoWebApp.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "DotNetToDoWebApp.dll"]
