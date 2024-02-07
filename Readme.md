We have two environments: The build environment, which is going to be an Unbuntu Linux on a 64-bit intel machine, and the target Environment, which is going to be a BeagleBone Play running Debian 11. The build environment is used to build the application, and the target environment is used to run the application.
## Build Environment Setup
1. Download the latest version of Ubuntu from https://ubuntu.com/download/desktop
2. Install .net 6.0 by using the Ubuntu packages available from https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu
3. Install the upgrade assistant for dotnet from https://dotnet.microsoft.com/en-us/platform/upgrade-assistant
4. Get the ECConet source code 
5. go to the source code and run the upgrade-assistant upgrade for each folder
6. Follow the instructions to upgrade the code to .net 6.0
7. Go back to the top ECCOnet director. Open the solution in Visual Studio 2022 and build the solution or run the command dotnet build

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
7. BeagleBone Play can also be accessed through ssh debian@192.168.7.2
### .Net 6.0 Installation
1. Go to the website https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install
2. Follow the instructions to install .Net 6.0 for Debian 11
3. To check if the installation was successful, open a terminal and type dotnet --version
