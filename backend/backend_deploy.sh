#!/bin/bash

cd ~/FairWorkly/backend

git pull origin main

docker stop fairworkly-api
docker rm fairworkly-api

docker build -t fairworkly-api .

docker run -d --name fairworkly-api -p 5680:5680 fairworkly-api

docker ps | grep fairworkly-api
