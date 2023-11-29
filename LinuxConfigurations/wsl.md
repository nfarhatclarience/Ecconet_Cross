## wsl 
n this part 2 we are going to let WSL ubuntu recognize the microntroller
First you need to install usbpipd
got their website and follow instructions
https://github.com/dorssel/usbipd-win

set ubuntu 22.04 as default
open a power shell as admin
wsl --set default Ubuntu-22.04

### open a wsl ubuntu and run
sudo apt install linux-tools-virtual hwdata sudo update-alternatives --install /usr/local/bin/usbip usbip ls /usr/lib/linux-tools/*/usbip | tail -n1 20

run apt upgrade
sudo apt upgrade

run alternatives command again
sudo update-alternatives --install /usr/local/bin/usbip usbip ls /usr/lib/linux-tools/*/usbip | tail -n1 20

## close wsl window
usbipd wsl list
#

#attach 2-2    2d03:0001  C3 Pro USB Interface                                          Not attached
usbipd wsl attach --busid 2-2

go back to ubuntu and run lsusb , you should see it
lssub


## additional Resources