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
    [RequireComponent(typeof(Joint))]
    public class JointStateWriter : MonoBehaviour
    {
        public int JointId;
        public enum JointTypes { continuous, revolute, prismatic };
        public JointTypes JointType;

        private Joint joint;
        
        private float newState; // deg or m
        private float prevState; // deg or m
        private bool doUpdate;

        private void Start()
        {
            joint = GetComponent<Joint>(); 
        }

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
            Vector3 anchor = transform.TransformPoint(joint.anchor);
            Vector3 axis = transform.TransformDirection(joint.axis);

            if (JointType == JointTypes.continuous || JointType == JointTypes.revolute)
                transform.RotateAround(anchor, axis, (prevState - newState) * Mathf.Rad2Deg);
            else if (JointType == JointTypes.prismatic)
                transform.Translate(axis * (prevState - newState));

            prevState = newState;
        }

        public void Write(float state)
        {
            newState = state ;
            doUpdate = true;
        }
    }
}