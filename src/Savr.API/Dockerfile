#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Savr.API/Savr.API.csproj", "Savr.API/"]
COPY ["Savr.Identity/Savr.Identity.csproj", "Savr.Identity/"]
COPY ["Savr.Application/Savr.Application.csproj", "Savr.Application/"]
COPY ["Savr.Domain/Savr.Domain.csproj", "Savr.Domain/"]
COPY ["Savr.Persistence/Savr.Persistence.csproj", "Savr.Persistence/"]
COPY ["Savr.Presentation/Savr.Presentation.csproj", "Savr.Presentation/"]
RUN dotnet restore "./Savr.API/./Savr.API.csproj"
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"


WORKDIR "/src/Savr.API"
RUN dotnet build "./Savr.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Savr.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savr.API.dll"]