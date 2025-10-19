#!/bin/bash
cd NordenAPI

# Install .NET 9.0 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.0
export PATH="$HOME/.dotnet:$PATH"

dotnet restore
dotnet build -c Release
dotnet publish -c Release -o ./publish
dotnet ./publish/NordenAPI.dll --urls http://0.0.0.0:$PORT
