#!/bin/bash
set -e

cd "$(dirname "$0")/.."

folderName="ToolInstallTest"

rm -rf $folderName && mkdir $folderName && cd $folderName

dotnet new nugetconfig
sed -i.bak '/<add/d' nuget.config && rm nuget.config.bak

echo ""
echo "Testing as local tool..."
dotnet new tool-manifest
dotnet tool install --add-source ../src/DotnetSops.CommandLine/bin/nupkg dotnet-sops --prerelease
dotnet tool run dotnet-sops -- --version
dotnet dotnet-sops --version
dotnet sops --version
dotnet sops download-sops
dotnet tool uninstall dotnet-sops

echo ""
echo "Testing as global tool..."
dotnet tool install --add-source ../src/DotnetSops.CommandLine/bin/nupkg dotnet-sops --prerelease --global
dotnet sops --version
dotnet sops download-sops
dotnet tool uninstall dotnet-sops --global

cd .. && rm -rf $folderName
