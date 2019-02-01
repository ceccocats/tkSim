# unity version
2018.2.7f1
https://forum.unity.com/threads/unity-on-linux-release-notes-and-known-issues.350256/page-2#post-3662605

# credits
MSVehicleSystem (Free)
https://github.com/vwaurich/VelodyneLidarUnitySimulation
https://github.com/tordanik/OSM2World
https://raw.githubusercontent.com/commaai/opendbc/master/tesla_can.dbc
https://github.com/howerj/dbcc.git

# Build map with OSM2World
export data
https://www.openstreetmap.org/export#map=18/44.62907/10.95028
convert to obj with OSM2World
refine with blender

# convert dbc
dbcc -k -u tesla_can.dbc

#camera
sudo apt install v4l2loopback-* 

then:
sudo modprobe v4l2loopback
mkfifo /tmp/tkcamera0
tail -c +1 -F /tmp/tkcamera0 | ffmpeg -i pipe:0 -vcodec rawvideo -pix_fmt yuyv422 -threads 0 -f v4l2 /dev/video1

#gps
socat -u -u pty,raw,echo=0,link=/tmp/ttyGPSin pty,raw,echo=0,link=/tmp/ttyGPS,b9600 
