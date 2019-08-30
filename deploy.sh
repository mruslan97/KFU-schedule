#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

# Create publish artifact
cd ./Schedule
dotnet publish -c Release src

# Build the Docker images
docker build -t mruslan97/kfuapp:$TAG src/bin/Release/netcoreapp2.2/publish/.
docker tag mruslan97/kfuapp:$TAG mruslan97/kfuapp:latest

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push mruslan97/kfuapp:$TAG
docker push mruslan97/kfuapp:latest