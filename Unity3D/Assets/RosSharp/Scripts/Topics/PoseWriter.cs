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
    public class PoseWriter : MonoBehaviour
    {
        public PoseSubscriber poseSubscriber;
        private Vector3 position;
        private Quaternion rotation;

        private bool doUpdate;

        private void Update()
        {
            if (doUpdate)
            {
                WriteUpdate();
                doUpdate = false;
            }
        }

        private void WriteUpdate()
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        public void Write(Vector3 _position, Quaternion _rotation)
        {
            position = _position.Ros2Unity();
            rotation = _rotation.Ros2Unity();
        }
    }
}