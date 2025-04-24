# base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish StockViewer.fsproj -c Release -o /app/publish

# final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StockViewer.dll"]
