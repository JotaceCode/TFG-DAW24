FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000
ENV ASPNETCORE_HTTP_PORTS=3000

COPY localhost.crt /app
COPY localhost.key /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services.API.Gateway/Services.API.Gateway.csproj", "Services.API.Gateway/"]
RUN dotnet restore "Services.API.Gateway/Services.API.Gateway.csproj"
COPY . .
WORKDIR "/src/Services.API.Gateway"
RUN dotnet build "Services.API.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services.API.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=https://+:3000;
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/localhost.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/app/localhost.key

ENTRYPOINT ["dotnet", "Services.API.Gateway.dll"]
