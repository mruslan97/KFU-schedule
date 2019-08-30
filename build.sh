#!/bin/bash
set -ev
cd ./Schedule
dotnet restore
dotnet test
dotnet build -c Release