#!/usr/bin/env bash

if [ "$1" == "--remove" ]; then
    rm -rfv $HOME/.local/share/FlappyBird
    rm -v $HOME/.local/share/applications/FlappyBird.desktop

    exit 0
fi

mkdir -pv $HOME/.local/share/applications
mkdir -pv $HOME/.local/share/FlappyBird

mv -v ./FlappyBird   $HOME/.local/share/FlappyBird
mv -v ./libraylib.so $HOME/.local/share/FlappyBird
mv -v ./Saves        $HOME/.local/share/FlappyBird
mv -v ./Assets       $HOME/.local/share/FlappyBird

cat << EOF > $HOME/.local/share/applications/FlappyBird.desktop
[Desktop Entry]
Type=Application
Name=Flappy Bird
Icon=${HOME}/.local/share/FlappyBird/icon.png
Exec=${HOME}/.local/share/FlappyBird/FlappyBird
Categories=Games;
Comment=Faithful Flappy Bird clone.
Terminal=false
EOF