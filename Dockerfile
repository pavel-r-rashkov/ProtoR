FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
ARG build_config=Release
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
RUN dotnet build --no-restore -c ${build_config}

# unit tests
FROM build AS unit-tests
LABEL protor-test=true
WORKDIR /app/UnitTests
ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "\"xunit;LogFilePath=../../TestResults/unit-tests.xml\""]

# publish
FROM build AS publish
ARG build_config=Release
LABEL protor-publish=true
WORKDIR /app/Web
RUN dotnet publish --no-build -c ${build_config} -o out

# run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=publish /app/Web/out ./
EXPOSE 80

RUN apt-get update \
  && apt-get install -y --no-install-recommends unzip \
  && apt-get install -y procps \
  && rm -rf /var/lib/apt/lists/* \
  && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

ENTRYPOINT ["dotnet", "Web.dll"]
