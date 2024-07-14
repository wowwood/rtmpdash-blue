#!/bin/bash

# Directory for png results
imgdir="/var/www/stats"

# Interface to monitor, or group of them in format "iface1+iface2"
interface="ens19"

# Making dir for results

mkdir -p $imgdir

#make images

vnstati -vs -i $interface -o $imgdir/vnstat1.png
vnstati -h -i $interface -o $imgdir/vnstat2.png
vnstati -d -i $interface -o $imgdir/vnstat3.png
vnstati -m -i $interface -o $imgdir/vnstat4.png

exit 0;
