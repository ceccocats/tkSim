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

using System;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace RosSharp.RosBridgeClient
{
    public class PointCloudPublisher : UnityPublisher<MessageTypes.Sensor.PointCloud2>
    {
        public VeloLidar lidar;
        private MessageTypes.Sensor.PointCloud message;
        private MessageTypes.Sensor.PointCloud2 finalMsg;
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
                header = new MessageTypes.Std.Header { frame_id = lidar.name }
            };
            finalMsg = new MessageTypes.Sensor.PointCloud2();
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

            convertPointCloudToPointCloud2();
            Publish(finalMsg);
        }

        private void convertPointCloudToPointCloud2() {
            finalMsg.header = message.header;
            finalMsg.width  = (uint) message.points.Length;
            finalMsg.height = (uint) 1;
            Array.Resize(ref finalMsg.fields, 3 + message.channels.Length);
            for(int i=0; i<finalMsg.fields.Length; i++) 
                finalMsg.fields[i] = new PointField();
            
            // Convert x/y/z to fields
            finalMsg.fields[0].name = "x"; finalMsg.fields[1].name = "y"; finalMsg.fields[2].name = "z";
            int offset = 0;
            // All offsets are *4, as all field data types are float32
            for (int d = 0; d < finalMsg.fields.Length; ++d, offset += 4)
            {
                finalMsg.fields[d].offset = (uint) offset;
                finalMsg.fields[d].datatype = MessageTypes.Sensor.PointField.FLOAT32;
                finalMsg.fields[d].count  = 1;
            }
            finalMsg.point_step = (uint) offset;
            finalMsg.row_step   = finalMsg.point_step * finalMsg.width;
            // Convert the remaining of the channels to fields
            for (int d = 0; d < message.channels.Length; ++d)
                finalMsg.fields[3 + d].name = message.channels[d].name;
            
            Array.Resize(ref finalMsg.data, (int) (message.points.Length * finalMsg.point_step));
            finalMsg.is_bigendian = false;  // @todo ?
            finalMsg.is_dense     = false;

            // Copy the data points
            for (int cp = 0; cp < message.points.Length; ++cp) {
                byte[] x_bytes = BitConverter.GetBytes(message.points[cp].x); 
                byte[] y_bytes = BitConverter.GetBytes(message.points[cp].y); 
                byte[] z_bytes = BitConverter.GetBytes(message.points[cp].z); 
                Array.Copy(x_bytes, 0, finalMsg.data, cp * finalMsg.point_step + finalMsg.fields[0].offset, sizeof(float));
                Array.Copy(y_bytes, 0, finalMsg.data, cp * finalMsg.point_step + finalMsg.fields[1].offset, sizeof(float));
                Array.Copy(z_bytes, 0, finalMsg.data, cp * finalMsg.point_step + finalMsg.fields[2].offset, sizeof(float));

                for (int d = 0; d < message.channels.Length; ++d) {
                    if (message.channels[d].values.Length == message.points.Length) {
                        byte[] c_bytes = BitConverter.GetBytes(message.channels[d].values[cp]); 
                        Array.Copy(c_bytes, 0, finalMsg.data, cp * finalMsg.point_step + finalMsg.fields[3 + d].offset, sizeof(float));
                    }
                }

            }

            return;
        }
    }
}
