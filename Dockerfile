FROM microsoft/dotnet:2.0.0-sdk AS build-env
WORKDIR /app
COPY . .
RUN mkdir ./release

RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.Common/PlugAndTrade.DieScheite.Client.Common.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.Common.Microsoft.Extensions.DependencyInjection/PlugAndTrade.DieScheite.Client.Common.Microsoft.Extensions.DependencyInjection.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.RabbitMQ/PlugAndTrade.DieScheite.Client.RabbitMQ.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.RabbitMQ.Microsoft.Extensions.DependencyInjection/PlugAndTrade.DieScheite.Client.RabbitMQ.Microsoft.Extensions.DependencyInjection.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.Console/PlugAndTrade.DieScheite.Client.Console.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.Console.Microsoft.Extensions.DependencyInjection/PlugAndTrade.DieScheite.Client.Console.Microsoft.Extensions.DependencyInjection.csproj \
  --output ../release
RUN dotnet pack \
  -c Release \
  --include-source \
  --include-symbols \
  PlugAndTrade.DieScheite.Client.AspNetCore/PlugAndTrade.DieScheite.Client.AspNetCore.csproj \
  --output ../release

RUN find ./release -name '*.nupkg' -not -name '*.symbols.nupkg' | xargs -i dotnet nuget push {} \
  -k 9df6908f-5140-4415-92f3-85fffdfabbaa \
  -s https://www.myget.org/F/plugandtrade-packages/api/v2/package \
  -ss https://www.myget.org/F/plugandtrade-packages/symbols/api/v2/package
