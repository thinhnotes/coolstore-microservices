FROM gitpod/workspace-dotnet

RUN dotnet tool install -g Microsoft.Tye --version "0.7.0-alpha.21279.2"
RUN export PATH="$PATH:/home/gitpod/.dotnet/tools"

# Install custom tools, runtimes, etc.
# For example "bastet", a command-line tetris clone:
# RUN brew install bastet
#
# More information: https://www.gitpod.io/docs/config-docker/
