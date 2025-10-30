# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the backend project file and restore dependencies
COPY OVOR.Server/OVOR.Server.csproj ./OVOR.Server/
RUN dotnet restore "OVOR.Server/OVOR.Server.csproj"

# Copy everything else and build
COPY . .
WORKDIR /src/OVOR.Server
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render assigns $PORT automatically
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

ENTRYPOINT ["dotnet", "OVOR.Server.dll"]
