FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /Solidarity
COPY . .
RUN apt update && \
	apt install -y procps && \
	apt install -y curl && \
	apt install unzip && \
	curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
ENTRYPOINT dotnet watch run