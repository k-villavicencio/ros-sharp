/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

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

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class JointStatePublisher : MonoBehaviour
    {
        public string Topic = "/joint_states";
        public string FrameId = "UnityFrameId";
        public JointStateReader[] jointStateReaders;

        private RosSocket rosSocket;
        private int publicationId;
        private int sequenceId;
        private SensorJointStates message;
        
        private void Start()
        {
            rosSocket = GetComponent<RosConnector>().RosSocket;
            publicationId = rosSocket.Advertize(Topic, "sensor_msgs/JointState");
            InitializeMessage();
        }

        private void InitializeMessage()
        {
            int jointStateLength = jointStateReaders.Length;
            message = new SensorJointStates();
            message.header.frame_id = FrameId;            
            message.name = new string[jointStateLength];
            message.position = new float[jointStateLength];
            message.velocity = new float[jointStateLength];
            message.effort = new float[jointStateLength];
        }

        private void Publish()
        {
            UpdateHeader();
            for (int i = 0; i < jointStateReaders.Length; i++)
                UpdateJointState(i);

            rosSocket.Publish(publicationId, message);
        }

        private void UpdateHeader()
        {
            message.header.seq = sequenceId++;
            // message.header.secs = tbd;
            // message.header.nsecs = tbd;
        }

        private void UpdateJointState(int i)
        {
            jointStateReaders[i].GetJointState(
                out message.name[i],
                out message.position[i],
                out message.velocity[i],
                out message.effort[i]);
        }
    }
}