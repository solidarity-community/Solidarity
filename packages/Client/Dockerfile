FROM node:16
RUN mkdir -p /Solidarity
WORKDIR /Solidarity
COPY package.json .
RUN npm install
COPY . .
EXPOSE 80
CMD ["npm", "start"]