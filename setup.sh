#!/usr/bin/env bash

if [ "$(uname)" == "Darwin" ]; then

    echo "Installing Glide for Darwin.\n"
    which -s brew
    if [[ $? != 0 ]] ; then
        echo "Installing via curl && shell."
        curl https://glide.sh/get | sh
    else
        echo "Brew detected, installing via 'brew install glide'."
        brew update
        brew install glide
    fi

    echo "Installing jet cli."
    brew cask install jet


    echo "Installing the Hugo static site generator."
    brew install hugo
    echo "Additional information on the Hugo installation."
    which hugo
    ls -l $( which hugo )
    hugo version

    echo "Installing Hugo Theme now."
    cd docs/hugo
    mkdir themes
    cd themes
    git clone --depth 1 --recursive git@github.com:crakjie/landing-page-hugo.git

elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then

    echo "Installing Glide for Linux."
    sudo add-apt-repository ppa:masterminds/glide && sudo apt-get update
    sudo apt-get install glide

    echo "Installing Jet CLI (re: Codeship > https://codeship.com)"
    curl -SLO "https://s3.amazonaws.com/codeship-jet-releases/1.16.0/jet-linux_amd64_1.16.0.tar.gz"
    sudo tar -xaC /usr/local/bin -f jet-linux_amd64_1.16.0.tar.gz
    sudo chmod +x /usr/local/bin/jet
    rm -rf jet-linux_amd64_1.16.0.tar.gz

    echo "Installing Hugo."
    cd ~
    wget https://github.com/spf13/hugo/releases/download/v0.18.1/hugo_0.18.1-64bit.deb
    sudo dpkg -i hugo*.deb

    echo "Additional information on the Hugo installation."
    which hugo
    ls -l $( which hugo )
    hugo version

    echo "Installing Hugo Theme now."
    cd docs/hugo
    mkdir themes
    cd themes
    git clone --depth 1 --recursive git@github.com:crakjie/landing-page-hugo.git

elif [ "$(expr substr $(uname -s) 1 10)" == "MINGW32_NT" ]; then
    echo "Windows detected, sorry. You're on your own. Cheers!"
    # Helpz! Anybody want to test out an installation and get this working on Windows?
    # Add glide > https://github.com/Adron/datadiluvium/issues/19
    # Add jet cli > https://github.com/Adron/datadiluvium/issues/18
fi

glide install

