# syntax=docker/dockerfile:1
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY global.json Directory.Build.props Home.slnx ./
COPY src/Home.Domain/*.csproj src/Home.Domain/
COPY src/Home.Application/*.csproj src/Home.Application/
COPY src/Home.Infrastructure/*.csproj src/Home.Infrastructure/
COPY src/Home.Web/*.csproj src/Home.Web/
RUN dotnet restore src/Home.Web/Home.Web.csproj -a $TARGETARCH
COPY src/ src/
RUN dotnet publish src/Home.Web/Home.Web.csproj -c Release -a $TARGETARCH --no-restore -o /app /p:UseAppHost=false

# Migration bundle (run as a Kubernetes Job before the rollout)
FROM build AS bundle
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --project src/Home.Infrastructure --startup-project src/Home.Infrastructure \
    --self-contained -r linux-x64 -o /efbundle --force

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080 DOTNET_EnableDiagnostics=0
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Smoehring.Home.Web.dll"]
