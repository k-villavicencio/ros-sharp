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
using System;

namespace RosSharp.RosBridgeClient
{

    public class JoyReceiver : MessageReceiver
    {
        public JoyButtonWriter[] joyButtonWriters;
        public JoyAxisWriter[] joyAxesWriters;
        public override Type MessageType { get { return (typeof(SensorJoy)); } }

        private SensorJoy message;
        private int joyButtonWritersLength;
        private int joyAxesWritersLength;
        private void Awake()
        {
            MessageReception += ReceiveMessage;
        }

        private void Start()
        {
            joyButtonWritersLength = joyButtonWriters.Length;
            joyAxesWritersLength = joyAxesWriters.Length;
        }

        private void ReceiveMessage(object sender, MessageEventArgs e)
        {            
            message = (SensorJoy)e.Message;
            int arrayLength = message.buttons.Length;            
            int I = (joyButtonWritersLength < arrayLength ? joyButtonWritersLength : arrayLength);
            for (int i = 0; i < I; i++)
                if (joyButtonWriters!=null)
                joyButtonWriters[i].Write(message.buttons[i]);

            arrayLength = message.buttons.Length;
            I = (joyAxesWritersLength < arrayLength ? joyAxesWritersLength : arrayLength);
            for (int i = 0; i < I; i++)
                joyAxesWriters[i].Write(message.axes[i]);
        }
    }
}
