FROM node AS build
WORKDIR /Solidarity
COPY . ./
RUN npm install && npm run build

FROM nginx
COPY --from=build /Solidarity/dist /usr/share/nginx/html
EXPOSE 8080