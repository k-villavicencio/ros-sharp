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
    public class PublicationTimer : MonoBehaviour
    {
        public event EventHandler PublicationEvent;
        public enum Timings { UnityFrameTime, UnityFixedTime, ClockTime }
        public Timings Timing;

        public int StepLength;

        private Thread waitForNextStep;
        private event EventHandler startNextStep;

        private int stepsSincePublication;
        private bool isApplicationRunning;

        public static PublicationTimer AddComponent(GameObject _gameObject, EventHandler publicationEventHandler, Timings timing = Timings.UnityFrameTime, int StepLength=1)
        {
            PublicationTimer publicationTimer = _gameObject.AddComponent<PublicationTimer>();
            publicationTimer.PublicationEvent += publicationEventHandler;
            publicationTimer.Timing = timing;
            publicationTimer.StepLength = StepLength;
            return publicationTimer;
        }

        private void Awake()
        {
            isApplicationRunning = true;

        }
        private void Start()
        {
            if (Timing == Timings.ClockTime)
                ClockTimeStep(null, null);
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
                && stepsSincePublication++ % StepLength == 0
                && PublicationEvent != null)
                PublicationEvent(this, EventArgs.Empty);
        }

        private void ClockTimeStep(object sender, EventArgs e)
        {
            if (!isApplicationRunning)
                return;

            startNextStep += ClockTimeStep;

            waitForNextStep = new Thread(WaitForNextStep);
            waitForNextStep.Start();

            if (PublicationEvent != null)
                PublicationEvent(this, EventArgs.Empty);
        }

        public void WaitForNextStep()
        {
            Thread.Sleep(StepLength);

            if (startNextStep != null)
                startNextStep(this, EventArgs.Empty);
        }

    }
}