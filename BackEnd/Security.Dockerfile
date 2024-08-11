FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000
ENV ASPNETCORE_HTTP_PORTS=3000
#ENV APP_CONNECTION_STRING="Data Source=192.168.1.94; Initial Catalog=Security; User Id=sa; Password=StrongPassword1!; TrustServerCertificate=true;"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services.API.Security/Services.API.Security.csproj", "Services.API.Security/"]
RUN dotnet restore "Services.API.Security/Services.API.Security.csproj"
COPY . .
WORKDIR "/src/Services.API.Security"
RUN dotnet build "Services.API.Security.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services.API.Security.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Services.API.Security.dll"]
