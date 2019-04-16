# To run

For development it is suggested to use docker-compose. `docker compose up` will
do the necessary ceremony

If one prefers to use pure docker and just want to see things run, then there
is a `./start.sh` which starts one redis container and one application
container (after building) and puts them on the same network. The application
container relies on redis being available on "tcp://redis:6379" which it will
be when they are on the same network and properly named. The application
container exposes on port localhost:8888, which the included `test.sh` script
runs some testcases against. If one starts on another port then `test.sh`
can be instructed to use a different hostname by providing it as first
argument. Suggestion session:

`bash ./start.sh ; bash ./test.sh`

And, to clean:

`bash ./stop.sh`

See ./start.sh for details on how to run, but essentially it is just:

1. docker network create radios
2. docker run -d --name redis --network radios redis
3. docker build -t radios .
4. docker run -d --network radios -p 127.0.0.1:8888:8888 --name radios radios
