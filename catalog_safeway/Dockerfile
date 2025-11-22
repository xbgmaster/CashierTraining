FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el csproj de la subcarpeta
COPY catalog_safeway/catalog_safeway.csproj catalog_safeway/

# Restaurar dependencias
RUN dotnet restore catalog_safeway/catalog_safeway.csproj

# Copiar todo el contenido del repo
COPY . .

# Entrar a la carpeta correcta del proyecto
WORKDIR /src/catalog_safeway

# Publicar
RUN dotnet publish catalog_safeway.csproj -c Release -o /app/publish


# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "catalog_safeway.dll"]
