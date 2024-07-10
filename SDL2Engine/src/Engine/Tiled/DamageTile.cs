using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Tiled
{
    public class DamageTile : Script
    {
        public int damage = 1000;

        public override void Start()
        {
            Collider? collider = GetComponent<Collider>();
            if(collider == null)
            {
                // create collider
                collider = BoxCollider.FromDrawableRect(gameObject);
            }

            BoxCollider? boxCollider = collider as BoxCollider;
            if(boxCollider != null)
            {
                // shrink collider a bit to avoid collision with adjacent tiles
                double shrinkW = 0.1 * boxCollider.box.w;
                double shrinkH = 0.1 * boxCollider.box.h;
                
                Rect newBox = new Rect(shrinkW, shrinkH, boxCollider.box.w - 2 * shrinkW, boxCollider.box.h - 2 * shrinkH);
                boxCollider.box = newBox;
            }
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            ITiledDamageable? damageable = other.GetComponent<ITiledDamageable>();

            if (damageable != null)
            {
                damageable.Damage(damage);
            }
        }
    }
}
