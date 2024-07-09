using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Tiled
{

    // a platform that is half the thickness of a normal tile
    // it can be passed through from the bottom
    public class ThinPlatform : Script
    {

        public override void Start()
        {
            var boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.box = new Rect(
                    boxCollider.box.x,
                    boxCollider.box.y,
                    boxCollider.box.w,
                    boxCollider.box.h / 2
                    );
            }
        }

        public override void OnPreCollision(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            var otherBody = other.GetComponent<PhysicsBody>();

            if (otherBody != null)
            {
                if (otherBody.Velocity.y < 0)
                {
                    collision.Cancel();
                }
            }
        }
    }
}
