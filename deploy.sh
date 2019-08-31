#!/bin/bash
set -ev

# Build the Docker images
chmod +x ./docker-build.sh
./docker-build.sh

# Login to Docker Hub and upload images
docker login -u="${DOCKER_USERNAME}" -p="${DOCKER_PASSWORD}" 
docker push mruslan97/kfuapp:dev
