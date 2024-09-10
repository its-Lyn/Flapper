#!/usr/bin/env bash

if [ "$1" == "--remove" ]; then
    rm -rfv $HOME/.local/share/FlappyBird
    rm -v $HOME/.local/share/applications/FlappyBird.desktop

    exit 0
fi

read -r -p "Before running, make sure you've ran \"dotnet publish -C Release -p:PublishSingleFile=true\". Continue? [Y/n] " RESPONSE
case $RESPONSE in
    [nN])
        exit 0
    ;;
esac

mkdir -pv $HOME/.local/share/applications
mkdir -pv $HOME/.local/share/FlappyBird

cp -rv FlappyBird/bin/Release/net8.0/linux-x64/publish/*   $HOME/.local/share/FlappyBird
rm -v $HOME/.local/share/FlappyBird/FlappyBird.pdb
cp -v Releases/icon.png $HOME/.local/share/FlappyBird

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