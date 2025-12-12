FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FileStorage/FileStorage.csproj", "FileStorage/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "FileStorage/FileStorage.csproj"

COPY . .
WORKDIR "/src/FileStorage"

FROM build AS publish
RUN dotnet publish "FileStorage.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.dll"]