#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

# Build the Docker images
chmod +x ./docker-build.sh
./docker-build.sh

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push mruslan97/kfuapp:dev
