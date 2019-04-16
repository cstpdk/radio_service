#!/bin/bash
URL=${1:-"http://localhost:8888"}

failed=""
success(){
  echo -e "\e[32m$1\e[0m"
}
fail() {
  echo -e "\e[31m$1\e[0m"
  failed="true"
}
t() {
  if [ "$1" != "$2" ] ; then
    fail "Test $3 failed\n$1 != $2"
  else
    success "Test $3 succeeded"
  fi
}

get_statuscode(){
  while read l ; do
    echo $l | grep -Eo 'HTTP/1.1 [0-9]+ .+' | sed -r 's|HTTP/1.1 ([0-9]+) .+|\1|'
  done
}

ACTUAL=$(curl -s --header 'Content-Type: application/json' "$URL"/radios/100 \
  -d '{"alias":"Radio100","allowed_locations":["CPH-1","CPH-2"]}')
DESIRED='{"id":100,"alias":"Radio100","allowed_locations":["CPH-1","CPH-2"],"location":null}'

t "$ACTUAL" "$DESIRED" "scenario 1.1"

ACTUAL=$(curl -s --header 'Content-Type: application/json' "$URL"/radios/101 \
  -d '{"alias":"Radio101","allowed_locations":["CPH-1","CPH-2","CPH-3"]}')
DESIRED='{"id":101,"alias":"Radio101","allowed_locations":["CPH-1","CPH-2","CPH-3"],"location":null}'

t "$ACTUAL" "$DESIRED" "scenario 1.2"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/100/location -d '{"location":"CPH-1"}' 2>&1 | get_statuscode)

t "$ACTUAL" "200" "scenario 1.3"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/101/location -d '{"location":"CPH-3"}' 2>&1 | get_statuscode)

t "$ACTUAL" "200" "scenario 1.4"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/100/location -d '{"location":"CPH-3"}' 2>&1 | get_statuscode)

t "$ACTUAL" "403" "scenario 1.5"

ACTUAL=$(curl -s --header 'Content-Type: application/json' $URL/radios/101/location)

t "$ACTUAL" '{"location":"CPH-3"}' "scenario 1.6 payload"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/101/location 2>&1 | get_statuscode)

t "$ACTUAL" "200" "scenario 1.6 code"

ACTUAL=$(curl -s --header 'Content-Type: application/json' $URL/radios/100/location)

t "$ACTUAL" '{"location":"CPH-1"}' "scenario 1.7 payload"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/100/location 2>&1 | get_statuscode)

t "$ACTUAL" "200" "scenario 1.7 code"

ACTUAL=$(curl -s --header 'Content-Type: application/json' "$URL"/radios/102 \
  -d '{"alias":"Radio102","allowed_locations":["CPH-1","CPH-3"]}')
DESIRED='{"id":102,"alias":"Radio102","allowed_locations":["CPH-1","CPH-3"],"location":null}'

t "$ACTUAL" "$DESIRED" "scenario 2.1 create"

ACTUAL=$(curl -s -v --header 'Content-Type: application/json' \
  $URL/radios/102/location 2>&1 | get_statuscode)

t "$ACTUAL" "404" "scenario 2.1 code"

if [ -n "$failed" ] ; then
  exit 1
fi
