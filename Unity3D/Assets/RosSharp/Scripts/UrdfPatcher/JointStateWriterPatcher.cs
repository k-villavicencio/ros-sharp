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

using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(JointStateSubscriber))]
    public class JointStateWriterPatcher : MonoBehaviour
    {
        public GameObject UrdfModel;
        

        public void Patch()
        {
            JointStateSubscriber jointStateSubscriber = GetComponent<JointStateSubscriber>();

            JointStateWriter jointStateWriter;
            JointStateWriter.JointTypes jointType;
            int jointID = 0;
            List<JointStateWriter> jointStateWriters = new List<JointStateWriter>();
            
            foreach (Transform child in UrdfModel.GetComponentsInChildren<Transform>())
            {
                child.DestroyImmediateIfExists<JointStateWriter>();

                if (isPatchable(child, out jointType))
                {
                    jointStateWriter = child.gameObject.AddComponent<JointStateWriter>();
                    jointStateWriter.JointType = jointType;
                    jointStateWriter.JointId = jointID++;
                    jointStateWriters.Add(jointStateWriter);
                    Debug.Log("JointStateWriter of Type " + jointType.ToString() + " with ID: " + jointStateWriter.JointId + "\nadded to GameObject \"" + child.name + "\".");
                }
            }
            jointStateSubscriber.JointStateWriters = jointStateWriters.ToArray();
        }

        private bool isPatchable(Transform child, out JointStateWriter.JointTypes jointType)
        {
            jointType = JointStateWriter.JointTypes.continuous;

            if (child.name.Contains("continuous Joint"))
                jointType = JointStateWriter.JointTypes.continuous;
            else if (child.name.Contains("revolute Joint"))
                jointType = JointStateWriter.JointTypes.revolute;
            else if (child.name.Contains("prismatic Joint"))
                jointType = JointStateWriter.JointTypes.prismatic;
            else
                return false;

            return true;
        }
    }
}
