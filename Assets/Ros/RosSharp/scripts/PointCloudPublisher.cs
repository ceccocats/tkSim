/*
© Siemens AG, 2018
Author: Berkay Alp Cakal (berkay_alp.cakal.ct@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace RosSharp.RosBridgeClient
{
    public class PointCloudPublisher : UnityPublisher<MessageTypes.Sensor.PointCloud>
    {
        public VeloLidar lidar;
        public string FrameId = "Unity";

        private MessageTypes.Sensor.PointCloud message;
        private float scanPeriod;
        private float previousScanTime = 0;
                

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            if (Time.realtimeSinceStartup >= previousScanTime + scanPeriod)
            {
                UpdateMessage();
                previousScanTime = Time.realtimeSinceStartup;
            }
        }

        private void InitializeMessage()
        {
            scanPeriod = lidar.period;

            message = new MessageTypes.Sensor.PointCloud
            {
                header = new MessageTypes.Std.Header { frame_id = FrameId }
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            
            lidar.scan();
            
            // allocate if necessary
            if(message.points.Length != lidar.points.Length) {
                Debug.Log("Allocate points: " + lidar.points.Length);
                message.points = new Point32[lidar.points.Length];
                for(int i=0; i<lidar.points.Length; i++) {
                    message.points[i] = new Point32(0,0,0);    
                }
                //message.channels = new ChannelFloat32[1];
                //message.channels[0].name = "intensity";
                //message.channels[0].values = new float[lidar.points.Length];
            }
            
            for(int i=0; i<lidar.points.Length; i++) {
                // unity2ros
                message.points[i].x =  lidar.points[i].z;
                message.points[i].y = -lidar.points[i].x;
                message.points[i].z =  lidar.points[i].y;
                //message.channels[0].values[i] = 0;
            }
            
            Publish(message);
        }
    }
}
