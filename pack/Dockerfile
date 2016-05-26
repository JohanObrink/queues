FROM node:6.2

WORKDIR /app

ADD package.json /app/package.json
RUN npm install --production

ADD ./ /app/

CMD npm start
