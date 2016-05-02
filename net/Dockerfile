FROM microsoft/aspnet:1.0.0-rc1-update1

RUN printf "deb http://ftp.us.debian.org/debian jessie main\n" >> /etc/apt/sources.list
RUN apt-get -qq update && apt-get install -qqy sqlite3 libsqlite3-dev && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY ./project.json /app/project.json
RUN ["dnu", "restore"]

COPY . /app

EXPOSE 5000
ENTRYPOINT ["dnx", "-p", "project.json", "web"]
