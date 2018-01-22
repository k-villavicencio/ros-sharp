/*
© CentraleSupelec, 2017
Author: Dr. Jeremy Fix (jeremy.fix@centralesupelec.fr)

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

// Adjustments to new Publication Timing and Execution Framework 
// © Siemens AG, 2018, Dr. Martin Bischoff (martin.bischoff@siemens.com)

using System;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class ImagePublisher : MonoBehaviour
    {
        public string Topic = "/image_raw/compressed";
        public string FrameId = "Camera";
        public ImageReader ImageReader;
        public PublicationTimer publicationTimer;

        [Range(0, 100)]
        public int qualityLevel = 50;

        private RosSocket rosSocket;
        private SensorCompressedImage message;
        private int publicationId;
        private int sequenceId;        

        private void Start()
        {            
            rosSocket = GetComponent<RosConnector>().RosSocket;
            publicationId = rosSocket.Advertize(Topic, "sensor_msgs/CompressedImage");
            publicationTimer.PublicationEvent += Publish;
            CreateMessage();
        }
        private void Publish(object sender, EventArgs e)
        {
            UpdateMessage();
            rosSocket.Publish(publicationId, message);
        }

        private void UpdateMessage()
        {
            message.header.Update();
            message.data = ImageReader.GetImage().EncodeToJPG(qualityLevel);
        }
        private void CreateMessage()
        {
            message = new SensorCompressedImage();
            message.header.frame_id = FrameId;
            message.format = "jpeg";

        }
    }
}
