FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Outboxer.Test.API/Outboxer.Test.API.csproj", "Outboxer.Test.API/"]
RUN dotnet restore "Outboxer.Test.API/Outboxer.Test.API.csproj"
COPY . .
WORKDIR "/src/Outboxer.Test.API"
RUN dotnet build "Outboxer.Test.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outboxer.Test.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outboxer.Test.API.dll"]
