FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /server
COPY . .

# Install VS Debugger
RUN apt update && \
	apt install -y procps && \
	apt install -y curl && \
	apt install unzip && \
	curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

ENTRYPOINT dotnet watch run
EXPOSE 80