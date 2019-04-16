#!/bin/bash
bash -c "./stop.sh"

docker network create radios
docker run -d --name redis --network radios redis

docker build -t radios .
until docker run -it --network radios appropriate/curl redis:6379 ; do
    sleep 1
    echo waiting for redis
done
docker run -d --network radios -p 127.0.0.1:8888:8888 --name radios radios

until docker run -it --network radios appropriate/curl radios:8888 ; do
    sleep 1
    echo waiting for radio service
done

echo "READY"
