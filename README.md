# tkSim
tk general porpose simulator.
Ready to simulate:
- car
- formula
- forklift (ROS)

### unity3d version: 2019.3.7f1

# getting started without ROS
- compile tkBrige
- chatlist commands in  NoRos folder

### supported sensors
- lidar
- GPS
- canbus
- camera

# getting started with ROS
ROS is the easiest way to start simulation on tkSim

### supported sensors
- lidar
- Odometry
- camera
  
### forklift scene
- open scene file:<br>
   `Assets/Ros/Scenes/industrial.unity`
- launch rosbrige in terminal:<br>
``` roslaunch rosbridge_server rosbridge_websocket.launch```
- start scene
- sensors are now published on ROS

![Unity3d](https://user-images.githubusercontent.com/11562617/78570956-f0f60700-7825-11ea-9fe0-a71c5a48e58b.png)
![rviz](https://user-images.githubusercontent.com/11562617/78570946-ef2c4380-7825-11ea-9e42-c0842df7ddde.png)