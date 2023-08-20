#!/bin/bash
set -e

cd "$(dirname "$0")/.."

configuration="Release"
timestamp=$(date +%s)
version="0.0.0-alpha.$timestamp"

echo "Packing..."
dotnet pack -p:Version=$version --configuration $configuration
