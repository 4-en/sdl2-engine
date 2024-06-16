﻿
using SDL2;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ShootEmUp.IngameUI;

namespace ShootEmUp.src.Entities
{
    internal class BombPowerUp : Script
    {
        public static Prototype CreateBombPowerUpPrototype()
        {
            var prototype = new Prototype("BombPowerUp");

            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/icons/Skillicon3_03.png");
            sprite.SetWorldSize(100, 100);



            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(35);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<ShieldPowerUp>();
            prototype.AddComponent<DestroyAndBombPowerUpOnCollision>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = false;

            return prototype;
        }

        private GameObject? player;
        public override void Start()
        {
            // set direction
            var body = GetComponent<PhysicsBody>();
            if (body != null)
                body.Velocity = new Vec2D(0, 1);

            player = Find("Player");
        }

    }



    class DestroyAndBombPowerUpOnCollision : Script
    {
        public override void OnCollisionEnter(CollisionPair collision)
        {
            var collisionName = collision.GetOther(this.gameObject).GetName();
            if (collisionName.Equals("Player"))
            {
                Destroy(this.gameObject);
                Console.WriteLine("Bomb PowerUp Collected");
                PlayerData.Instance.BombCount++;

            }
        }

        public override void Update()
        {
        }

    }
}