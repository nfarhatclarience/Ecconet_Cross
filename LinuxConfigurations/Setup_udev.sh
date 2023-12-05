#!/bin/bash
# Enable robust error handling and shell behavior
set -Eeuo pipefail
shopt -s nullglob

# Define the rule file path
SIB_CODE3_RULE_FILE="/etc/udev/rules.d/99-usb-c3-pro.rules"

# Check for root privileges
if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root" 1>&2
    exit 1
fi

# Create the udev rule
echo 'SUBSYSTEM=="usb", ATTRS{idVendor}=="2d03", ATTRS{idProduct}=="0001", MODE="0666", SYMLINK+="sib_c3_pro_usb"' > $SIB_CODE3_RULE_FILE

# Reload udev rules
udevadm control --reload-rules
udevadm trigger

# Confirm completion
echo "Udev rule created and applied successfully."
