# Flapper

A faithful, nearly 1:1 clone of the video game classic Flappy Bird!

> [!CAUTION]
> I, do not, IN ANY WAY, claim I'm the original creator of this game! <br>
> I've merely recreated a hood classic. <br>
> All credit goes to .Gears Studio!

## Installation
Instructions on installing the game.
### Using releases page (Recommended)
Click on the [Releases page](https://github.com/its-Lyn/Flapper/releases)! <br>
Once there, simply click on your OS's Release!

> ![TIP]
> For Linux based distributions, you can run `setup.sh` to create a desktop entry!

### Compiling yourself (Advanced)
If you do not wish to use the release pages, you can also compile the game yourself.

#### Pre-requisities
To compile the game you require a few apps; `git`, `dotnet-sdk` and `dotnet-runtime`

```bash
# Ubuntu/Fedora
sudo apt/dnf install git dotnet-sdk-8.0 dotnet-runtime-8.0

# Arch based distributions
sudo pacman -S git dotnet-sdk dotnet-runtime
```

#### Compilation
Now, we can start installing the game! <br>
Begin by opening your favourite `terminal emulator`!

```
git clone https://github.com/its-Lyn/Flapper
cd Flapper
```

All that is left now is to compile the game. For Linux systems, you can also run `install.sh`, it will create a desktop entry for you. <br>
Run the following command to compile the game.
```bash
dotnet publish -c Release -p:PublishSingleFile=true
```

## Uninstalling
Uninstalling Flapper is as easy as running one command!
```bash
# If you used the Releases Page
./setup.sh --remove

# If you compiled yourself
./install.sh --remove
```

## Credits
Flappy Bird's existence is only possible due to .Gears Studio!

## License
MIT