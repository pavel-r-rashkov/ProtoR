#!/bin/bash

if [[ "$(docker images --filter "label=protor-build=true" -q | head -1 2> /dev/null)" != "" ]]; then
	docker rmi $(docker images --filter "label=protor-build=true" -q | head -1)
fi

if [[ "$(docker images --filter "label=protor-test=true" -q | head -1 2> /dev/null)" != "" ]]; then
	docker rmi $(docker images --filter "label=protor-test=true" -q | head -1)
fi

if [[ "$(docker images --filter "label=protor-publish=true" -q | head -1 2> /dev/null)" != "" ]]; then
	docker rmi $(docker images --filter "label=protor-publish=true" -q | head -1)
fi
