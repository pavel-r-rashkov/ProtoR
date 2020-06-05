FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
ARG build_config=Release
LABEL protor-build=true
WORKDIR /app

# copy csproj files
COPY *.sln .
COPY src/ProtoR.Web/*.csproj ./src/ProtoR.Web/
COPY src/ProtoR.Domain/*.csproj ./src/ProtoR.Domain/
COPY src/ProtoR.Application/*.csproj ./src/ProtoR.Application/
COPY src/ProtoR.Infrastructure/*.csproj ./src/ProtoR.Infrastructure/
COPY tests/ProtoR.Domain.UnitTests/*.csproj ./tests/ProtoR.Domain.UnitTests/
COPY tests/ProtoR.DataAccess.IntegrationTests/*.csproj ./tests/ProtoR.DataAccess.IntegrationTests/
COPY tests/ProtoR.ComponentTests/*.csproj ./tests/ProtoR.ComponentTests/
COPY Directory.Build.props .
RUN dotnet restore

# copy rest of files
COPY src/ProtoR.Web/. ./src/ProtoR.Web/
COPY src/ProtoR.Domain/. ./src/ProtoR.Domain/
COPY src/ProtoR.Application/. ./src/ProtoR.Application/
COPY src/ProtoR.Infrastructure/. ./src/ProtoR.Infrastructure/
COPY tests/ProtoR.Domain.UnitTests/. ./tests/ProtoR.Domain.UnitTests/
COPY tests/ProtoR.DataAccess.IntegrationTests/. ./tests/ProtoR.DataAccess.IntegrationTests/
COPY tests/ProtoR.ComponentTests/. ./tests/ProtoR.ComponentTests/
COPY default.ruleset .
COPY tests.ruleset .
RUN dotnet build --no-restore -c ${build_config}

# unit tests
FROM build AS unit-tests
LABEL protor-test=true
WORKDIR /app/tests/ProtoR.Domain.UnitTests
ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "\"xunit;LogFilePath=../../TestResults/unit-tests.xml\"", "/p:CollectCoverage=true", "/p:CoverletOutput=\"../../TestResults/coverage.xml\"", "/p:CoverletOutputFormat=cobertura"]

# integration tests
FROM build AS integration-tests
LABEL protor-test=true
RUN apt-get update && apt-get install -y openjdk-11-jre-headless
WORKDIR /app/tests/ProtoR.DataAccess.IntegrationTests
ENV COMPlus_EnableAlternateStackCheck=1
ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "\"xunit;LogFilePath=../../TestResults/integration-tests.xml\""]

# component tests
FROM build AS component-tests
LABEL protor-test=true
RUN apt-get update && apt-get install -y openjdk-11-jre-headless
WORKDIR /app/tests/ProtoR.ComponentTests
ENV COMPlus_EnableAlternateStackCheck=1
ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "\"xunit;LogFilePath=../../TestResults/component-tests.xml\""]

# publish
FROM build AS publish
ARG build_config=Release
LABEL protor-publish=true
WORKDIR /app/src/ProtoR.Web
RUN dotnet publish --no-build -c ${build_config} -o out

# run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
RUN apt-get update && apt-get install -y openjdk-11-jre-headless
WORKDIR /app
COPY --from=publish /app/src/ProtoR.Web/out ./
EXPOSE 80

RUN if [ "$build_config" = "Debug" ] ; then \
  apt-get update \
  && apt-get install -y --no-install-recommends unzip \
  && apt-get install -y procps \
  && rm -rf /var/lib/apt/lists/* \
  && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg \
  ; fi

ENV COMPlus_EnableAlternateStackCheck=1
ENTRYPOINT ["dotnet", "ProtoR.Web.dll"]
