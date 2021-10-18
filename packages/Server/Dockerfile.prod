FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /Solidarity
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o dist

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /Solidarity
COPY --from=build /Solidarity/dist .
EXPOSE 80
EXPOSE 443
RUN apt update && \
	apt install -y procps && \
	apt install -y curl && \
	apt install unzip && \
	curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
ENTRYPOINT ["dotnet", "Solidarity.dll"]