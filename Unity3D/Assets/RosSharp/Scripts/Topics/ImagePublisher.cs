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

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class ImagePublisher : MonoBehaviour
    {
        public string Topic = "/image_raw/compressed";
        public string frameId = "camera";

        [Range(0, 100)]
        public int qualityLevel = 50;

        private RosSocket rosSocket;
        private int publicationId;
        private SensorCompressedImage message;
        private int sequenceId;
        
        private void Start()
        {            
            rosSocket = GetComponent<RosConnector>().RosSocket;
            publicationId = rosSocket.Advertize(Topic, "sensor_msgs/CompressedImage");
            message = new SensorCompressedImage();
            sequenceId = 0;
        }

        public void Publish(Texture2D texture2D)
        {
            message.header.frame_id = frameId;
            message.header.seq = sequenceId++;
            message.format = "jpeg";
            message.data = texture2D.EncodeToJPG(qualityLevel);
            rosSocket.Publish(publicationId, message);
        }
    }
}
