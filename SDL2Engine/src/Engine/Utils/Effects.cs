using System;
using System.Collections.Generic;

namespace SDL2Engine.Utils
{
    public class CameraShaker : Script
    {
        private double shakeDuration = 3f;
        private double shakeMagnitude = 0.7f;
        private Camera? camera;



        public override void Start()
        {
            camera = scene?.GetCamera();
        }

        public override void Update()
        {
            // TODO: Implement camera shake
            // First: make camera use gameobject position
        }
    }
}
