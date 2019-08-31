#!/bin/bash
set -eux
find . \( -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" \) -print0 \
    | tar -cvf projectfiles.tar --null -T -

docker build -t=mruslan97/kfuapp:dev ./

rm projectfiles.tar