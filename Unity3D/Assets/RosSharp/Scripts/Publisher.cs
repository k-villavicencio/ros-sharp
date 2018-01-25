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
using System.Threading;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class Publisher : MonoBehaviour
    {
        public string Topic = "/image_raw/compressed";
        public string Type = "sensor_msgs/CompressedImage";

        public MessageProvider MessageProvider;

        public event EventHandler PublicationEvent;

        public enum Timings { UnityFrameTime, UnityFixedTime, ClockTime }
        public Timings Timing;

        public int SkipFrames;
        public float Timestep;
        private int timestep { get { return (int)(Mathf.Round(Timestep * 1000)); } }
        private int skipFrames { get { return SkipFrames + 1; } }

        private Thread waitForNextStep;

        private RosSocket rosSocket;
        private int publicationId;

        private int stepsSincePublication;
        private bool isApplicationRunning;

        private void Awake()
        {
            isApplicationRunning = true;
        }
        private void Start()
        {
            if (Timing == Timings.ClockTime)
                ClockTimeStep();

            rosSocket = GetComponent<RosConnector>().RosSocket;
            publicationId = rosSocket.Advertize(Topic, MessageProvider.MessageType);
            PublicationEvent += ReadMessage;
        }

        private void ReadMessage(object sender, EventArgs e)
        {
            Debug.Log("check");
            MessageProvider.OnMessageRequest(e);
            MessageProvider.MessageReady += Publish;
        }
        private void Publish(object sender, MessageReadyEventArgs e)
        {
            MessageProvider.MessageReady -= Publish;
            rosSocket.Publish(publicationId, e.Message);
        }

        private void LateUpdate()
        {
            UpdateStep(Timings.UnityFrameTime);
        }

        private void FixedUpdate()
        {
            UpdateStep(Timings.UnityFixedTime);
        }
        private void OnApplicationQuit()
        {
            isApplicationRunning = false;
        }

        private void UpdateStep(Timings UpdateTiming)
        {
            if (Timing == UpdateTiming
                && stepsSincePublication++ % skipFrames == 0
                && PublicationEvent != null)
                PublicationEvent(this, EventArgs.Empty);
        }

        private void ClockTimeStep()
        {
            if (!isApplicationRunning)
                return;

            if (PublicationEvent != null)
                PublicationEvent(this, EventArgs.Empty);

            waitForNextStep = new Thread(WaitForNextStep);
            waitForNextStep.Start();
        }

        public void WaitForNextStep()
        {
            Thread.Sleep(timestep);
            ClockTimeStep();
        }
    }
}