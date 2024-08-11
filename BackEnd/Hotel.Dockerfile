FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000
ENV ASPNETCORE_HTTP_PORTS=3000
#ENV APP_CONNECTION_STRING="mongodb://192.168.1.94:27017"
#ENV APP_DATABASE="MongoDBHotel"
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services.API.Hotel/Services.API.Hotel.csproj", "Services.API.Hotel/"]
RUN dotnet restore "Services.API.Hotel/Services.API.Hotel.csproj"
COPY . .
WORKDIR "/src/Services.API.Hotel"
RUN dotnet build "Services.API.Hotel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services.API.Hotel.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Services.API.Hotel.dll"]
