FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Gateway/Gateway.csproj", "Gateway/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Gateway/Gateway.csproj"

COPY . .
WORKDIR "/src/Gateway"

FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]