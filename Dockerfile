#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server.CourseCenter/Server.CourseCenter.csproj", "Server.CourseCenter/"]
RUN dotnet restore "Server.CourseCenter/Server.CourseCenter.csproj"
COPY . .
WORKDIR "/src/Server.CourseCenter"
RUN dotnet build "Server.CourseCenter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.CourseCenter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.CourseCenter.dll"]