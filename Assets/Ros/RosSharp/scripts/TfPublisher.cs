using UnityEngine;

namespace RosSharp.RosBridgeClient 
{
    public class TfPublisher : UnityPublisher<MessageTypes.Tf2.TFMessage>
    {
        public string frame_id;
        public GameObject[] list;
        private MessageTypes.Tf2.TFMessage message;
        
        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message             = new MessageTypes.Tf2.TFMessage();
            message.transforms  = new MessageTypes.Geometry.TransformStamped[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                message.transforms[i] = new MessageTypes.Geometry.TransformStamped();
                message.transforms[i].header.frame_id = frame_id;
            }
        }
        
        private void UpdateMessage()
        {           
            for (int i = 0; i < list.Length; i++)
            {
                // fill header
                message.transforms[i].header.Update();
                message.transforms[i].child_frame_id        = list[i].name;

                // fill trasform
                message.transforms[i].transform.translation = GetGeometryVector3(list[i].transform.localPosition.Unity2Ros());
                message.transforms[i].transform.rotation    = GetGeometryQuaternion(list[i].transform.localRotation.Unity2Ros());
            }
            
            Publish(message);
        }

        private MessageTypes.Geometry.Vector3 GetGeometryVector3(Vector3 position)
        {
            MessageTypes.Geometry.Vector3 geometryVector3 = new MessageTypes.Geometry.Vector3();
            geometryVector3.x = position.x;
            geometryVector3.y = position.y;
            geometryVector3.z = position.z;
            return geometryVector3;
        }

        private MessageTypes.Geometry.Quaternion GetGeometryQuaternion(Quaternion quaternion)
        {
            MessageTypes.Geometry.Quaternion geometryQuaternion = new MessageTypes.Geometry.Quaternion();
            geometryQuaternion.x = quaternion.x;
            geometryQuaternion.y = quaternion.y;
            geometryQuaternion.z = quaternion.z;
            geometryQuaternion.w = quaternion.w;
            return geometryQuaternion;
        }
    }
}
