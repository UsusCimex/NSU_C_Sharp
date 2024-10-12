# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy the build output
COPY --from=build /app/out .

# Copy data files
COPY /data /data

# Expose port
EXPOSE 80

ENTRYPOINT ["dotnet", "DeveloperService.dll"]
