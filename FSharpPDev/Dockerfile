FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FSharpPDev/FSharpPDev.fsproj", "FSharpPDev/"]
RUN dotnet restore "FSharpPDev/FSharpPDev.fsproj"
COPY . .
WORKDIR "/src/FSharpPDev"
RUN dotnet build "FSharpPDev.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FSharpPDev.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FSharpPDev.dll"]
