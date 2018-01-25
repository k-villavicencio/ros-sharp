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
    [RequireComponent(typeof(Camera))]
    public class ImageProvider : MessageProvider
    {
        protected override Message GetMessage()
        {
            return message;
        }
        public string FrameId = "Camera";
        public int resolutionWidth = 640;
        public int resolutionHeight = 480;
        [Range(0, 100)]
        public int qualityLevel = 50;

        private bool isMessageRequested;

        private SensorCompressedImage message;
        private Texture2D texture2D;
        private Camera _camera;
        private Rect rect;
        private RenderTexture renderTexture;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            texture2D = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
            rect = new Rect(0, 0, resolutionWidth, resolutionHeight);
            renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
            MessageRequest += RequestMessage;
            CreateMessage();
            Debug.Log(message.Type);
        }

        private void RequestMessage(object sender, EventArgs e)
        {
            isMessageRequested = true;
        }
        private void Update()
        {
            if (isMessageRequested)
            {
                Debug.Log("Reading Message in Frame: " + Time.frameCount + " at Time: " + Time.realtimeSinceStartup);
                UpdateMessage();
                isMessageRequested = false;
            }
        }

        private void CreateMessage()
        {
            message = new SensorCompressedImage();
            message.header.frame_id = FrameId;
            message.format = "jpeg";

        }
        private void UpdateMessage()
        {
            message.header.Update();
            message.data = GetImage().EncodeToJPG(qualityLevel);
            OnMessageReady(new MessageReadyEventArgs(message));
        }
        private Texture2D GetImage()
        {
            _camera.targetTexture = renderTexture;
            _camera.Render();
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(rect, 0, 0);
            _camera.targetTexture = null;
            RenderTexture.active = null;
            return texture2D;
        }
    }
}
