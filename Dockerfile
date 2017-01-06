FROM golang:1.7.3-alpine
RUN mkdir /app /local_gopath
ADD . /app/
WORKDIR /app
RUN export GOPATH=/local_gopath && \
  apk update && apk upgrade && \
  apk add --no-cache git && \
  go get github.com/xcrowley/fake && \
  go build -o main .
CMD ["/app/main"]
