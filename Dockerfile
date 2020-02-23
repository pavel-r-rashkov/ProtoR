FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
ARG build_config=Release
LABEL protor-build=true
WORKDIR /app

# copy csproj files
COPY *.sln .
COPY src/ProtoR.Web/*.csproj ./src/ProtoR.Web/
COPY src/ProtoR.Domain/*.csproj ./src/ProtoR.Domain/
COPY tests/ProtoR.Domain.UnitTests/*.csproj ./tests/ProtoR.Domain.UnitTests/
COPY Directory.Build.props .
RUN dotnet restore

# copy rest of files
COPY src/ProtoR.Web/. ./src/ProtoR.Web/
COPY src/ProtoR.Domain/. ./src/ProtoR.Domain/
COPY tests/ProtoR.Domain.UnitTests/. ./tests/ProtoR.Domain.UnitTests/
COPY default.ruleset .
RUN dotnet build --no-restore -c ${build_config}

# unit tests
FROM build AS unit-tests
LABEL protor-test=true
WORKDIR /app/tests/ProtoR.Domain.UnitTests
ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "\"xunit;LogFilePath=../../../TestResults/unit-tests.xml\"", "/p:CollectCoverage=true", "/p:CoverletOutput=\"../../../TestResults/coverage.xml\"", "/p:CoverletOutputFormat=cobertura"]

# publish
FROM build AS publish
ARG build_config=Release
LABEL protor-publish=true
WORKDIR /app/ProtoR.Web
RUN dotnet publish --no-build -c ${build_config} -o out

# run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=publish /app/ProtoR.Web/out ./
EXPOSE 80

RUN if [ "$build_config" = "Debug" ] ; then \
  apt-get update \
  && apt-get install -y --no-install-recommends unzip \
  && apt-get install -y procps \
  && rm -rf /var/lib/apt/lists/* \
  && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg \
  ; fi

ENTRYPOINT ["dotnet", "Web.dll"]
