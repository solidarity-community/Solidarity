FROM node:16
RUN mkdir -p /Solidarity
WORKDIR /Solidarity
COPY package.json .
RUN npm install
COPY . .
EXPOSE 8080
CMD ["npm", "start"]