# Utiliza la imagen base de .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establece el directorio de trabajo en la aplicación
WORKDIR /app

# Copia el archivo csproj y restaura las dependencias
COPY ./src /app
RUN dotnet restore /app/WebApi/WebApi.csproj

# Copia el resto del código fuente y compila la aplicación
COPY . .
RUN dotnet publish -c Release -o out /app/WebApi/WebApi.csproj

# Crea la imagen final usando la imagen ligera de .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expone el puerto en el que la aplicación escucha
EXPOSE 80

# Define la entrada al ejecutar el contenedor
ENTRYPOINT ["dotnet", "WebApi.dll"]
