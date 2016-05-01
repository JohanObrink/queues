FROM node:5.11

WORKDIR /app

ADD package.json /app/package.json
RUN npm install --production

ADD ./lib /app/lib

CMD npm start
