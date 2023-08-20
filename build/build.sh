#!/bin/bash
set -e

cd "$(dirname "$0")/.."

configuration="Release"
timestamp=$(date +%s)
version="0.0.0-alpha.$timestamp"

echo "Cleaning..."
dotnet clean --configuration $configuration

echo "Validating format..."
dotnet format --no-restore --verify-no-changes

echo "Building..."
dotnet build --configuration $configuration

echo "Testing..."
dotnet test --no-build --configuration $configuration

echo "Packing..."
dotnet pack -p:Version=$version --configuration $configuration
