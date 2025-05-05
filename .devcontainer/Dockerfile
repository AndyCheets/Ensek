FROM mcr.microsoft.com/dotnet/sdk:8.0

# Create a non-root user
RUN useradd -m vscode && \
    apt-get update && \
    apt-get install -y sqlite3 libsqlite3-dev 
    #&& \ chown -R vscode /workspace

USER vscode

WORKDIR /workspace

# Optional: install EF CLI tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/home/vscode/.dotnet/tools"
