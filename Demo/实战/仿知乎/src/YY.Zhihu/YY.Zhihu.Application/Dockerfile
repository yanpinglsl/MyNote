#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YY.Zhihu.Application/YY.Zhihu.Application.csproj", "YY.Zhihu.Application/"]
RUN dotnet restore "./YY.Zhihu.Application/./YY.Zhihu.Application.csproj"
COPY . .
WORKDIR "/src/YY.Zhihu.Application"
RUN dotnet build "./YY.Zhihu.Application.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YY.Zhihu.Application.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YY.Zhihu.Application.dll"]