# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["OVOR.sln", "./"]
COPY ["OVOR.Server/OVOR.Controller.csproj", "OVOR.Server/"]
COPY ["OVOR.Models/OVOR.Models.csproj", "OVOR.Models/"]
COPY ["OVOR.Repo/OVOR.Repo.csproj", "OVOR.Repo/"]
COPY ["OVOR.Services/OVOR.Services.csproj", "OVOR.Services/"]

# Restore dependencies
RUN dotnet restore "OVOR.sln"

# Copy everything and publish
COPY . .
WORKDIR "/src/OVOR.Server"
RUN dotnet publish -c Release -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Bind to port 10000 by default
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OVOR.Controller.dll"]
