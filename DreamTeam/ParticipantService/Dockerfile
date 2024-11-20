FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build
WORKDIR /src

# Копируем файлы проекта
COPY ./ParticipantService/ParticipantService.csproj ./ParticipantService/
COPY ./Shared/Shared.csproj ./Shared/

# Восстанавливаем зависимости
RUN dotnet restore ./ParticipantService/ParticipantService.csproj

# Копируем весь код
COPY . .

# Публикуем проект
WORKDIR /src/ParticipantService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0.8 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ParticipantService.dll"]
