#!/bin/bash
./.devcontainer/dotnet-install.sh -version 6.0.201 
pushd /workspaces
dotnet tool install -g Microsoft.Tye --version 0.11.0-alpha.22111.1
popd

chmod 0755 ./.devcontainer/dotnet-install.sh
./.devcontainer/dotnet-install.sh -version 2.1.816
