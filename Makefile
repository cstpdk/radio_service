run-test:
	-docker-compose down
	docker-compose up -d redis prod
	until curl localhost:8888 ; do sleep 1 ; echo 'waiting' ; done
	bash ./test.sh
