FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install NativeAOT build prerequisites
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev

WORKDIR /source

COPY . .
RUN dotnet publish src/Hive.CLI/ -c release -r linux-x64 -o /app

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 80
EXPOSE 587

ENTRYPOINT ["/app/Hive.CLI"]