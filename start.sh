#!/bin/bash
cd NordenAPI

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore --verbosity quiet

# Publish the application
echo "Publishing application..."
dotnet publish -c Release -o ./publish --verbosity quiet

# Set the port for Railway
export PORT=${PORT:-8080}

# Start the application
echo "Starting NordenAPI on port $PORT..."
dotnet ./publish/NordenAPI.dll --urls http://0.0.0.0:$PORT
