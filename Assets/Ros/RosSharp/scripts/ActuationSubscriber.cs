using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ActuationSubscriber : UnitySubscriber<MessageTypes.Geometry.Vector3>
    {
        public GameObject forklift;
        public bool manual = false;

        private float previousRealTime, steer, speed;
        private bool isMessageReceived;
        private NewCarController m_Car; // the car controller we want to use

        protected override void Start()
        {
            base.Start();

            // get the car controller
            m_Car               = forklift.GetComponent<NewCarController>();
            previousRealTime    = Time.realtimeSinceStartup;
        }

        protected override void ReceiveMessage(MessageTypes.Geometry.Vector3 message)
        {
            speed = (float) message.x;
            steer = (float) message.y;

            isMessageReceived = true;
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.M))
                manual = !manual;

            if (manual) 
            {
                // pass the input to the car!
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

#if !MOBILE_INPUT
                float handbrake = Input.GetAxis("Jump");
                m_Car.Move(h, v, v, handbrake);
            
#else
                m_Car.Move(h, v, v, 0f);
#endif
            }
            else 
            {
                if (isMessageReceived) 
                {
                    // move the fork
                    m_Car.Move(steer, speed, speed, 0f);

                    previousRealTime  = Time.realtimeSinceStartup;
                    isMessageReceived = false;
                } else {
                    float deltaTime = Time.realtimeSinceStartup - previousRealTime;

                    // if no message is received in one second, stop the forklift
                    if (deltaTime > 1.0f) 
                        m_Car.Move(0f, 0f, 0f, 0f);
                }           
            }
        }
    }
}