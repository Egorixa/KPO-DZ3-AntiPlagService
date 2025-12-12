FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Frontend/Frontend.csproj", "Frontend/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Frontend/Frontend.csproj"

COPY . .
WORKDIR "/src/Frontend"

FROM build AS publish
RUN dotnet publish "Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /app/publish/wwwroot .

COPY Frontend/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80