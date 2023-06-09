#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Learn/Learn.csproj", "Learn/"]
RUN dotnet restore "Learn/Learn.csproj"
COPY . .
RUN dotnet build "Learn/Learn.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Learn/Learn.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .