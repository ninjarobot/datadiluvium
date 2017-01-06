FROM golang:1.7.3-alpine
RUN mkdir /app
ADD . /app/
WORKDIR /app
RUN apk update && apk upgrade && \
  apk add --no-cache git && \
  go get github.com/icrowley/fake && \
  go build -o main .
CMD ["/app/main"]
