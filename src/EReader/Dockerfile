ARG BASE_IMAGE=mcr.microsoft.com/dotnet/aspnet:5.0.5-buster-slim-arm32v7
FROM ${BASE_IMAGE} AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/EReader/EReader.csproj", "src/EReader/"]
RUN dotnet restore "src/EReader/EReader.csproj"
COPY . .
WORKDIR "/src/src/EReader"
RUN dotnet build "EReader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EReader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY src/EReader/mu88_root_CA.crt /usr/local/share/ca-certificates/mu88_root_CA.crt
ENTRYPOINT ["dotnet", "EReader.dll"]