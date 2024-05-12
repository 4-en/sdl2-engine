using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong
{


    public class PortalScript : Script
    {
        public class ConnectedPortal : Script
        {
            public PortalScript? portalScript;
            public ConnectedPortal? other;
            public bool onCooldown = false;
            public double lastTeleport = 0;
            public double cooldownTime = 1;


            public override void Start()
            {
                GameObject? parent = gameObject.GetParent();
                if(parent == null)
                {
                    return;
                }

                portalScript = parent.GetComponent<PortalScript>();
                other = portalScript?.GetOtherPortal(this);
            }

            public override void Update()
            {
                if(this.portalScript == null || this.other == null)
                {
                    return;
                }

                if (onCooldown && Time.time - lastTeleport > cooldownTime)
                {
                    onCooldown = false;
                }

            }

            public void Teleport(GameObject obj)
            {
                if (this.portalScript == null || this.other == null || onCooldown)
                {
                    return;
                }

                Vec2D pos = obj.GetPosition();
                Vec2D otherPos = other.gameObject.GetPosition();
                Vec2D thisPos = gameObject.GetPosition();
                Vec2D diff = pos - thisPos;
                Vec2D newPos = otherPos + diff;
                obj.SetPosition(newPos);
                onCooldown = true;
                other.onCooldown = true;
                lastTeleport = Time.time;
                other.lastTeleport = Time.time;
            }

            public override void OnCollisionEnter(CollisionPair collision)
            {
                if (this.portalScript == null || this.other == null || onCooldown)
                {
                    return;
                }
                
                GameObject otherObject = collision.obj1 == gameObject ? collision.obj2 : collision.obj1;
                if (otherObject.GetName() != "Ball")
                {
                    return;
                }

                Teleport(otherObject);


            }
        }

        private ConnectedPortal[] connectedPortals = new ConnectedPortal[2];
        public Color? color = null;

        public ConnectedPortal? GetOtherPortal(ConnectedPortal portal)
        {
            if (connectedPortals[0] == portal)
            {
                return connectedPortals[1];
            }
            else if (connectedPortals[1] == portal)
            {
                return connectedPortals[0];
            }
            return null;
        }

        public override void Start()
        {
            if(color == null)
            {
                var bytes = new byte[3];
                random.NextBytes(bytes);
                color = new Color(bytes[0], bytes[1], bytes[2]);
            }

            var portal1 = Component.CreateWithGameObject<ConnectedPortal>();
            var rect1 = portal1.Item1.AddComponent<DrawableRect>();
            rect1.color = color.Value;
            rect1.SetRect(new Rect(0, 0, 150, 150));
            rect1.anchorPoint = AnchorPoint.Center;
            var coll1 = BoxCollider.FromDrawableRect(portal1.Item1);
            if(coll1 != null)
            {
                coll1.IsTrigger = true;
            }

            var portal2 = Component.CreateWithGameObject<ConnectedPortal>();
            var rect2 = portal2.Item1.AddComponent<DrawableRect>();
            rect2.color = color.Value;
            rect2.SetRect(new Rect(0, 0, 150, 150));
            rect2.anchorPoint = AnchorPoint.Center;
            var coll2 = BoxCollider.FromDrawableRect(portal2.Item1);
            if (coll2 != null)
            {
                coll2.IsTrigger = true;
            }

            connectedPortals[0] = portal1.Item2;
            connectedPortals[1] = portal2.Item2;
            gameObject.AddChild(portal1.Item1);
            gameObject.AddChild(portal2.Item1);

            Vec2D minPos = new Vec2D(300, 100);
            Vec2D maxPos = new Vec2D(1920 - 300, 1080 - 100);
            Vec2D pos1 = new Vec2D(random.Next((int)minPos.x, (int)maxPos.x), random.Next((int)minPos.y, (int)maxPos.y));
            Vec2D pos2 = new Vec2D(random.Next((int)minPos.x, (int)maxPos.x), random.Next((int)minPos.y, (int)maxPos.y));
            portal1.Item1.SetPosition(pos1);
            portal2.Item1.SetPosition(pos2);
        }

        public override void Update()
        {

        }
    }
}
