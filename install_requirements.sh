#!/bin/bash

echo "Installing SDL2 and other required libraries..."

# Update package lists
sudo apt update

# Install SDL2 development library
sudo apt install -y libsdl2-dev

# Install SDL2_image development library
sudo apt install -y libsdl2-image-dev

# Install SDL2_ttf development library
sudo apt install -y libsdl2-ttf-dev

echo "SDL2 and its extensions SDL2_image and SDL2_ttf have been installed."

# Install git submodules
git submodule update --init --recursive

echo "Git submodules have been installed."