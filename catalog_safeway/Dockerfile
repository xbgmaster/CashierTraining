FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["catalog_safeway/catalog_safeway.csproj", "catalog_safeway/"]
RUN dotnet restore "catalog_safeway/catalog_safeway.csproj"
COPY . .
WORKDIR "/src/catalog_safeway"
RUN dotnet publish "catalog_safeway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "catalog_safeway.dll"]
