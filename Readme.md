# Setup 
## Preliminaries are converting from .net framework 4.5 to .net core (.net 8.0)
1. Get the ECCOnet source code 
5. go to the source code and run the upgrade-assistant upgrade for each folder
6. Follow the instructions to upgrade the code to .net 8.0
7. Go back to the top ECCOnet director. Open the solution in Visual Studio 2022 and build the solution or run the command dotnet build
## Build Environment Setup
1. Windows or Ubuntu Linux with .net 8.0 ( I expect to work with any Linux flavor) 
1. Download the latest version of Ubuntu from https://ubuntu.com/download/desktop
2. Install .net 8.0 by using the Ubuntu packages available from https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

## Target Environment Setup
### Flashing the BeagleBone Play
1. Download the latest image from https://beagleboard.org/latest-images
2. Download the latest version of balenaEtcher from https://www.balena.io/etcher/
3. Insert the SD card into the computer
4. Open balenaEtcher and select the image you downloaded
5. Select the SD card and click on Flash
6. Once the flashing is complete, insert the SD card into the BeagleBone Play
### Basic Setup
1. Connect the Antennas as instructed in the user manual of the beagleBone Play
2. Connect the BeagleBone Play to a HDMI monitor
3. Connect the BeagloneBone PLay to a keyboard 
4. Go to Applications -> Internet -> wpa_gui
5. Select the network you want to connect to and enter the password
6. Open a terminal and do sudo apt update
7. BeagleBone Play can also be accessed through  (usb-c)
   ```
   ssh debian@192.168.7.2
   ```
   or
   ```
   ssh debian@192.168.1.162
   ```
    via wireless. 
### .Net 8.0 SDK Installation
1. Go to the website https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install
2. Follow the instructions to install .Net 6.0 for Debian 11
3. To check if the installation was successful, open a terminal and type
   ```
   dotnet --version
   ```
### Build Ecconet Cross 
1. After you clone the directory
2. go to Ecconet_cross/Demo1/
3. ```
   dotnet publish --runtime linux-arm64 --self-contained -p:PublishReadyToRun=true
   ```
### Move the Release to Single Board computer
1.
