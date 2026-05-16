FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Course_Work_Magazine.API/Course_Work_Magazine.API.csproj"

RUN dotnet publish "Course_Work_Magazine.API/Course_Work_Magazine.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Course_Work_Magazine.API.dll"]