#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MortagePlan/MortagePlan.csproj", "MortagePlan/"]
RUN dotnet restore "MortagePlan/MortagePlan.csproj"
COPY . .
WORKDIR "/src/MortagePlan"
RUN dotnet build "MortagePlan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MortagePlan.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MortagePlan.dll"]
