#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Packages
# COPY ["Package.Database/", "Package.Database/"]
# WORKDIR "/src/Package.Database"
# RUN dotnet restore "Package.Database/Package.Database.csproj"
# RUN dotnet build "Package.Database.csproj" -c Release -o /app/build

WORKDIR /src

# Copy AuthenticationService
# COPY ["AuthenticationService/", "AuthenticationService/"]
COPY . .
RUN dotnet restore "AuthenticationService/AuthenticationService.csproj"
WORKDIR "/src/AuthenticationService"
RUN dotnet build "AuthenticationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]