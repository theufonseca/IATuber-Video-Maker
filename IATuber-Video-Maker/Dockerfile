#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IATuber-Video-Maker/IATuber-Video-Maker.csproj", "IATuber-Video-Maker/"]
RUN dotnet restore "IATuber-Video-Maker/IATuber-Video-Maker.csproj"
COPY . .
WORKDIR "/src/IATuber-Video-Maker"
RUN dotnet build "IATuber-Video-Maker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IATuber-Video-Maker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IATuber-Video-Maker.dll"]