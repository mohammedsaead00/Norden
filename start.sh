#!/bin/bash
cd NordenAPI
dotnet restore --verbosity quiet
dotnet publish -c Release -o ./publish --verbosity quiet
dotnet ./publish/NordenAPI.dll --urls http://0.0.0.0:$PORT
