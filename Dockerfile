FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TestBackendTask.App/TestBackendTask.App.csproj"
WORKDIR "/src/TestBackendTask.App"
RUN dotnet build "TestBackendTask.App.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "TestBackendTask.App.csproj" -c Release -o /app/publish

FROM base AS final
ENV Http__Host=+
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestBackendTask.App.dll"]
EXPOSE 8080
