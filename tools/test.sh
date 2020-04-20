#!/bin/bash

full_path=$(realpath $0)
tools_dir_path=$(dirname $full_path)
solution_dir_path=$(dirname $tools_dir_path)
[[ -d $solution_dir_path/TestResults ]] || mkdir $solution_dir_path/TestResults

# Fix path for Windows FS
if [[ "$OSTYPE" == "msys" ]]; then
  solution_dir_path=$(echo "$solution_dir_path" | sed 's/^\///' | sed 's/\//\\/g' | sed 's/^./\0:/')      
fi

# Run unit tests
docker build -t protor-unit-tests --target unit-tests .
echo -e '\e[1mWaiting for unit tests to finish...\e[0m'
docker run --name protor-unit-tests protor-unit-tests
docker wait protor-unit-tests

# Copy unit test results
echo -e '\e[1mCopying unit tests results from container...\e[0m'
docker cp protor-unit-tests:/app/TestResults $solution_dir_path/

# Clean unit tests container
echo -e '\e[1mRemoving unit tests container...\e[0m'
docker rm protor-unit-tests

# Run integration tests
docker build -t protor-integration-tests --target integration-tests .
echo -e '\e[1mWaiting for integration tests to finish...\e[0m'
docker run --name protor-integration-tests protor-integration-tests
docker wait protor-integration-tests

# Copy integration test results
echo -e '\e[1mCopying integration tests results from container...\e[0m'
docker cp protor-integration-tests:/app/TestResults $solution_dir_path/

# Clean integration tests container
echo -e '\e[1mRemoving integration tests container...\e[0m'
docker rm protor-integration-tests
