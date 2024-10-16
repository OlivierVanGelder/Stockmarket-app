FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0-amd64 AS build-env
WORKDIR /app

COPY ./ ./
WORKDIR /app/Presentation
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0-distroless-extra
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Presentation.dll"]