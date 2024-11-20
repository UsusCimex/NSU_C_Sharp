FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build
WORKDIR /src

# Копируем файлы проекта
COPY ./HRDirectorService/HRDirectorService.csproj ./HRDirectorService/
COPY ./Shared/Shared.csproj ./Shared/

# Восстанавливаем зависимости
RUN dotnet restore ./HRDirectorService/HRDirectorService.csproj

# Копируем весь код
COPY . .

# Публикуем проект
WORKDIR /src/HRDirectorService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0.8 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HRDirectorService.dll"]
