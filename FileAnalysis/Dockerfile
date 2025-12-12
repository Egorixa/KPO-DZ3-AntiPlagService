FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FileAnalysis/FileAnalysis.csproj", "FileAnalysis/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "FileAnalysis/FileAnalysis.csproj"

COPY . .
WORKDIR "/src/FileAnalysis"

FROM build AS publish
RUN dotnet publish "FileAnalysis.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileAnalysis.dll"]