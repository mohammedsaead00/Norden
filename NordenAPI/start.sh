#!/bin/bash
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o ./publish
dotnet ./publish/NordenAPI.dll --urls http://0.0.0.0:$PORT
