FROM microsoft/dotnet:2.1-sdk AS build
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
RUN dotnet build

# test
FROM build AS test
LABEL protor-test=true
WORKDIR /app/UnitTests
RUN dotnet test --logger "xunit;LogFilePath=../../TestResults/UnitTests.xml"

# publish
FROM build AS publish
LABEL protor-publish=true
WORKDIR /app/Web
RUN dotnet publish -c Release -o out

# run
FROM microsoft/dotnet:2.1-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/Web/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Web.dll"]
