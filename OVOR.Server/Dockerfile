FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all csproj files
COPY OVOR.sln .
COPY OVOR.Server/*.csproj ./OVOR.Server/
COPY OVOR.Models/*.csproj ./OVOR.Models/
COPY OVOR.Repo/*.csproj ./OVOR.Repo/
COPY OVOR.Services/*.csproj ./OVOR.Services/

RUN dotnet restore "OVOR.sln"

# Copy everything else
COPY . .

WORKDIR /src/OVOR.Server
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080
ENTRYPOINT ["dotnet", "OVOR.Server.dll"]
