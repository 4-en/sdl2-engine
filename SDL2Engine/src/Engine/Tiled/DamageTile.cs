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
