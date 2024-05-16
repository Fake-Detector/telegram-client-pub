FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5176;https://+:7176
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="123456"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/telegramclient.pfx
COPY ["telegramclient.pfx", "/https/telegramclient.pfx"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fake.Detection.Telegram.Client.Api/Fake.Detection.Telegram.Client.Api.csproj", "src/Fake.Detection.Telegram.Client.Api/"]
COPY ["src/Fake.Detection.Telegram.Client.Bll/Fake.Detection.Telegram.Client.Bll.csproj", "src/Fake.Detection.Telegram.Client.Bll/"]
COPY ["src/Fake.Detection.Telegram.Client.Integration/Fake.Detection.Telegram.Client.Integration.csproj", "src/Fake.Detection.Telegram.Client.Integration/"]
RUN dotnet nuget add source "https://nuget.pkg.github.com/Fake-Detector/index.json" -n github -u <USER> -p <PASSWORD> --store-password-in-clear-text
RUN dotnet restore "src/Fake.Detection.Telegram.Client.Api/Fake.Detection.Telegram.Client.Api.csproj"
COPY . .
WORKDIR "/src/src/Fake.Detection.Telegram.Client.Api"
RUN dotnet build "Fake.Detection.Telegram.Client.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fake.Detection.Telegram.Client.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fake.Detection.Telegram.Client.Api.dll"]
