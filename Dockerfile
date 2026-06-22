FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY building-blocks/ building-blocks/
COPY src/ src/
RUN dotnet restore src/Users.API/Users.API.csproj
RUN dotnet publish src/Users.API/Users.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Users.API.dll"]
