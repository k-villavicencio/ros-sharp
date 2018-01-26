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
    public class PoseSubscriber : MonoBehaviour
    {
        public string Topic;
        public enum MessageTypes { GeometryPose, GeometryPoseWithCovariance, NavigationOdometry };
        public MessageTypes MessageType;
        public PoseWriter poseWriter;

        private RosSocket rosSocket;

        public void Start()
        {
           // string rosMessageType = MessageReflector.MessageDictionary[System.Type.GetType(MessageType.ToString())];
            //Debug.Log(rosMessageType);
            rosSocket = GetComponent<RosConnector>().RosSocket;            
            rosSocket.Subscribe(Topic, "fixme", Receive);
        }

        private void Receive(Message message)
        {
            poseWriter.Write(GetPosition(message), GetRotation(message));            
        }

        private Vector3 GetPosition(Message message)
        {
            switch (MessageType)
            {
                case MessageTypes.GeometryPose:
                    {
                        GeometryPose geometryPose = ((GeometryPose)message);
                        return new Vector3(
                            geometryPose.position.x,
                            geometryPose.position.y,
                            geometryPose.position.z);
                    }
                case MessageTypes.GeometryPoseWithCovariance:
                    {
                        GeometryPoseWithCovariance geometryPoseWithCovariance
                            = (GeometryPoseWithCovariance)message;
                        return new Vector3(
                            geometryPoseWithCovariance.pose.position.x,
                            geometryPoseWithCovariance.pose.position.y,
                            geometryPoseWithCovariance.pose.position.z);
                    }

                case MessageTypes.NavigationOdometry:
                    {
                        NavigationOdometry navigationOdometry
                            = (NavigationOdometry)message;
                        return new Vector3(
                            navigationOdometry.pose.pose.position.x,
                            navigationOdometry.pose.pose.position.y,
                            navigationOdometry.pose.pose.position.z);
                    }
                default:
                    return Vector3.zero;
            }
        }

        private Quaternion GetRotation(Message message)
        {
            switch (MessageType)
            {
                case MessageTypes.GeometryPose:
                    {
                        GeometryPose geometryPose = ((GeometryPose)message);
                        return new Quaternion(
                            geometryPose.orientation.x,
                            geometryPose.orientation.y,
                            geometryPose.orientation.z,
                            geometryPose.orientation.w);
                    }
                case MessageTypes.GeometryPoseWithCovariance:
                    {
                        GeometryPoseWithCovariance geometryPoseWithCovariance
                            = (GeometryPoseWithCovariance)message;
                        return new Quaternion(
                            geometryPoseWithCovariance.pose.orientation.x,
                            geometryPoseWithCovariance.pose.orientation.y,
                            geometryPoseWithCovariance.pose.orientation.z,
                            geometryPoseWithCovariance.pose.orientation.w);
                    }

                case MessageTypes.NavigationOdometry:
                    {
                        NavigationOdometry navigationOdometry
                            = (NavigationOdometry)message;
                        return new Quaternion(
                            navigationOdometry.pose.pose.orientation.x,
                            navigationOdometry.pose.pose.orientation.y,
                            navigationOdometry.pose.pose.orientation.z,
                            navigationOdometry.pose.pose.orientation.w);
                    }
                default:
                    return Quaternion.identity;
            }
        }
    }

}

