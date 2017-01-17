#!/usr/bin/env bash

if [ "$(uname)" == "Darwin" ]; then
    echo "Installing Glide for Darwin.\n"

    which -s brew
    if [[ $? != 0 ]] ; then
        echo "Installing via curl && shell.\n"
        curl https://glide.sh/get | sh
    else
        echo "Brew detected, installing via 'brew install glide'."
        brew update
        brew install glide
    fi
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
    echo "Installing Glide for Linux."
    sudo add-apt-repository ppa:masterminds/glide && sudo apt-get update
    sudo apt-get install glide
elif [ "$(expr substr $(uname -s) 1 10)" == "MINGW32_NT" ]; then
    echo "Windows detected, sorry. You're on your own. Cheers!"
fi

glide install