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
    [RequireComponent(typeof(JointStateProvider))]
    public class JointStateReaderPatcher : MonoBehaviour
    {
        public GameObject UrdfModel;
        
        public void Patch()
        {
            JointStateProvider jointStateProvider = GetComponent<JointStateProvider>();

            JointStateReader jointStateReader;
            JointStateReader.JointTypes jointType;
            int jointID = 0;
            List<JointStateReader> jointStateReaders = new List<JointStateReader>();
            
            foreach (Transform child in UrdfModel.GetComponentsInChildren<Transform>())
            {
                child.DestroyImmediateIfExists<JointStateReader>();

                if (isPatchable(child, out jointType))
                {
                    jointStateReader = child.gameObject.AddComponent<JointStateReader>();
                    jointStateReader.JointId = jointID++;
                    jointStateReader.JointType = jointType;

                    jointStateReaders.Add(jointStateReader);
                    Debug.Log("JointStateReader of Type " + jointType.ToString() + " with ID: " + jointStateReader.JointId + "\nadded to GameObject \"" + child.name + "\".");
                }
            }
            jointStateProvider.JointStateReaders = jointStateReaders.ToArray();
        }

        private bool isPatchable(Transform child, out JointStateReader.JointTypes jointType)
        {
            jointType = JointStateReader.JointTypes.continuous;

            if (child.name.Contains("continuous Joint"))
                jointType = JointStateReader.JointTypes.continuous;
            else if (child.name.Contains("revolute Joint"))
                jointType = JointStateReader.JointTypes.revolute;
            else if (child.name.Contains("prismatic Joint"))
                jointType = JointStateReader.JointTypes.prismatic;
            else
                return false;

            return true;
        }
    }
}
