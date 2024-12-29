FROM node:22 as client
WORKDIR /client
COPY ./packages/Client/ .
RUN npm ci
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS server
WORKDIR /server
COPY ./packages/Server/*.csproj .
RUN dotnet restore
COPY ./packages/Server .
RUN dotnet test
RUN dotnet publish -c Release -o dist

FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY --from=server /server/dist ./server
COPY --from=client /client/dist ./server/wwwroot

WORKDIR /server
EXPOSE 80
ENTRYPOINT ["dotnet", "Solidarity.dll"]