#!/bin/bash
set -e

docker stop fairworkly-api || true
docker rm fairworkly-api || true

docker build -t fairworkly-api .

docker run -d --name fairworkly-api -p 5680:5680 fairworkly-api

docker ps | grep fairworkly-api