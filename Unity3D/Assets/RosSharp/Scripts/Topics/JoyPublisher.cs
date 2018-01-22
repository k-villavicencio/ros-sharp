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

using System;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class JoyPublisher : MonoBehaviour
    {
        public string Topic;
        public string FrameId = "Unity";
        public PublicationTimer publicationTimer;

        public JoyAxisReader[] JoyAxisReaders;
        public JoyButtonReader[] JoyButtonReaders;

        private RosSocket rosSocket;
        private SensorJoy message;
        private int publicationId;
        private int sequenceId;       

        private void Start()
        {
            rosSocket = GetComponent<RosConnector>().RosSocket;
            publicationId = rosSocket.Advertize(Topic, "sensor_msgs/Joy");
            publicationTimer.PublicationEvent += Publish;
            CreateMessage();
        }

        private void Publish(object sender, EventArgs e)
        {
            UpdateMessage();
            rosSocket.Publish(publicationId, message);
        }

        private void CreateMessage()
        {
            message = new SensorJoy();
            message.header.frame_id = FrameId;
            message.axes = new float[JoyAxisReaders.Length];
            message.buttons = new int[JoyButtonReaders.Length];
        }

        private void UpdateMessage()
        {
            message.header.Update();

            for (int i = 0; i < JoyAxisReaders.Length; i++)
                message.axes[i] = JoyAxisReaders[i].Value;

            for (int i = 0; i < JoyAxisReaders.Length; i++)
                message.buttons[i] = (JoyButtonReaders[i].Value ? 1 : 0);
        }
    }
}
