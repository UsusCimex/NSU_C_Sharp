# Используем SDK .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build
WORKDIR /app

# Копируем файл проекта и восстанавливаем зависимости
COPY DreamTeam.csproj ./
RUN dotnet restore

# Копируем остальной код
COPY . ./

# Сборка приложения
RUN dotnet publish -c Release -o out

# Используем runtime образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0.8
WORKDIR /app
COPY --from=build /app/out ./

# Копируем файлы CSV
COPY TeamLeads.csv ./
COPY Juniors.csv ./

# Запуск приложения
ENTRYPOINT ["dotnet", "DreamTeam.dll"]
