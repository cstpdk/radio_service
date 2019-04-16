#!/bin/bash
URL=${1:-"http://localhost:8888"}

CREATE1=$(curl --header 'Content-Type: application/json' "$URL"/radios/100 \
  -d '{"alias":"Radio100","allowed_locations":["CPH-1","CPH-2"]}')
CREATE1_DESIRED_RESULT='{"id":100,"alias":"Radio100","allowed_locations":["CPH-1","CPH-2"],"location":null}'
if [ "$CREATE1" != "$CREATE1_DESIRED_RESULT" ] ; then
  echo "$CREATE1 != $CREATE1_DESIRED_RESULT"
  exit 1
fi
