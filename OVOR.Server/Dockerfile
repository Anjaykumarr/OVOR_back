# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files first
COPY OVOR.Server/OVOR.Controller.csproj ./OVOR.Server/
COPY OVOR.Models/*.csproj ./OVOR.Models/
COPY OVOR.Repo/*.csproj ./OVOR.Repo/
COPY OVOR.Services/*.csproj ./OVOR.Services/

# Restore dependencies
RUN dotnet restore "OVOR.Server/OVOR.Controller.csproj"

# Copy everything and build
COPY . .
WORKDIR /src/OVOR.Server
RUN dotnet publish "OVOR.Controller.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render automatically provides $PORT
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

ENTRYPOINT ["dotnet", "OVOR.Controller.dll"]
