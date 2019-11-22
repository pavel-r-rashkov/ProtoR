FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
LABEL protor-build=true
WORKDIR /app

# copy csproj files
COPY src/*.sln .
COPY src/Web/*.csproj ./Web/
COPY src/UnitTests/*.csproj ./UnitTests/
RUN dotnet restore

# copy rest of files
COPY src/Web/. ./Web/
COPY src/UnitTests/. ./UnitTests/
RUN dotnet build --no-restore -c Release

# unit tests
FROM build AS unit-tests
LABEL protor-test=true
WORKDIR /app/UnitTests
ENTRYPOINT ["dotnet", "test", "-c", "Release", "--no-build", "--logger", "\"xunit;LogFilePath=../../TestResults/unit-tests.xml\""]

# publish
FROM build AS publish
LABEL protor-publish=true
WORKDIR /app/Web
RUN dotnet publish --no-build -c Release -o out

# run
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app
COPY --from=publish /app/Web/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Web.dll"]
