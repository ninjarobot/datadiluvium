FROM golang:1.7.4-alpine

ENV GOPATH /go

RUN mkdir /app && \
    apk add --update curl && \
    rm -rf /var/cache/apk/*

ADD . /app/
WORKDIR /app
RUN apk update && apk upgrade && \
  apk add --no-cache git && \
  curl https://glide.sh/get | sh

RUN glide up && go get github.com/icrowley/fake && \
  go build
CMD ["/app/datadiluvium"]
