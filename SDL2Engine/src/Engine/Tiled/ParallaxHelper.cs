using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Tiled
{
    internal class ParallaxHelper : Script
    {
        public double parallaxX = 1.0;
        public double parallaxY = 1.0;
        private Vec2D truePosition = new Vec2D(0, 0);
        private Vec2D lastPosition = new Vec2D(0, 0);

        private Camera? camera;
        // lastCameraPosition is used to determine if the camera has moved
        // saves some calculations if the camera hasn't moved
        private Vec2D lastCameraPosition = new Vec2D(0, 0);

        public override void Start()
        {
            camera = GetCamera();
            lastCameraPosition = camera.GetPosition();
            truePosition = gameObject.GetPosition();
            lastPosition = truePosition;
        }

        public override void Update()
        {
            if(camera == null)
            {
                camera = GetCamera();
                if(camera == null)
                {
                    return;
                }
            }

            Vec2D cameraPosition = camera.GetPosition();
            if(cameraPosition == lastCameraPosition)
            {
                return;
            }

            Vec2D position = gameObject.GetPosition();
            truePosition = truePosition + (position - lastPosition);
            
            // calculate parallax
            double xMovementFactor = 1 - parallaxX;
            double yMovementFactor = 1 - parallaxY;
            Vec2D cameraCenter = camera.GetPosition() + camera.GetVisibleSize() / 2;
            Vec2D parallaxMovement = (cameraCenter) * new Vec2D(xMovementFactor, yMovementFactor);
            lastPosition = truePosition + parallaxMovement;
            lastCameraPosition = cameraPosition;

            gameObject.SetPosition(lastPosition);
        }
    }
}
