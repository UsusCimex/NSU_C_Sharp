FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build
WORKDIR /src

# Копируем файлы проекта
COPY ./HRManagerService/HRManagerService.csproj ./HRManagerService/
COPY ./Shared/Shared.csproj ./Shared/

# Восстанавливаем зависимости
RUN dotnet restore ./HRManagerService/HRManagerService.csproj

# Копируем весь код
COPY . .

# Публикуем проект
WORKDIR /src/HRManagerService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0.8 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY ./TeamLeads.csv ./Juniors.csv /app/
ENTRYPOINT ["dotnet", "HRManagerService.dll"]
