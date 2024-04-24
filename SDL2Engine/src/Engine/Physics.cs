﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{

    // defines physical properties of an object
    // for example, mass, velocity, friction, etc.
    public class PhysicsBody : Component
    {
        // if true, objects with this component can be moved when colliding with other objects
        private bool isMovable = false;
        private Vec2D velocity = new Vec2D();
        private double mass = 1.0;
        private double bounciness = 1.0;
        private double friction = 0.0;
        private double drag = 0.0;

    }

    // defines physical shape of an object and handles collision detection
    public class Collider : Component
    {
        // TODO: Implement Collider class
        // Base class for all colliders

        // imlpement collision functions for all collider types
        // for example, BoxCollider and BoxCollider, BoxCollider and CircleCollider, etc.
        public virtual bool CollidesWith(Collider other)
        {
            if (other is CircleCollider)
            {
                return CollidesWith((CircleCollider)other);
            }
            if (other is EdgeCollider)
            {
                return CollidesWith((EdgeCollider)other);
            }   
            if (other is BoxCollider)
            {
                return CollidesWith((BoxCollider)other);
            }   
            return false;
        }

        public virtual bool CollidesWith(BoxCollider other)
        {
            //circlecollider and boxcollider
            if (other is CircleCollider)
            {
                return false;
            }
            //edgecollider and boxcollider
            if (other is EdgeCollider)
            {
                return false;
            }   
            //boxcollider and boxcollider
            if (other is BoxCollider)
            {
                return false;
            }   
            return false;
        }

        public virtual bool CollidesWith(CircleCollider other)
        {
            if (other is CircleCollider)
            {
                return false;
            }
            if (other is EdgeCollider)
            {
                return false;
            }
            if (other is BoxCollider)
            {
                return false;
            }
            return false;
        }

        public virtual bool CollidesWith(EdgeCollider other)
        {
            if (other is CircleCollider)
            {
                return false;
            }  
            if (other is EdgeCollider)
            {
                return false;
            }
            if (other is BoxCollider)
            {
                return false;
            }
            return false;
        }
    }

    // defines a pair of objects that are colliding so they can be resolved later
    public class CollisionPair
    {
        public GameObject obj1;
        public GameObject obj2;

        public CollisionPair(GameObject obj1, GameObject obj2)
        {
            this.obj1 = obj1;
            this.obj2 = obj2;
        }
    }

    // defines a box collider
    // a box collider is a rectangle that can be used to detect collisions
    public class BoxCollider : Collider
    {

        //create a box collider at the position of the game object
        public Rect box { get; set; }

        public BoxCollider(Rect box)
        {
            this.box = box;
        }
        public BoxCollider(int width, int height)
        {
            this.box = new Rect(0, 0, width, height);
        }
        public BoxCollider()
        {
            this.box = new Rect(50, 50, 50, 50);
        }


        public void UpdateColliderPosition(Vec2D newPosition)
        {
            this.box.x = newPosition.x;
            this.box.y = newPosition.y;
        }

        public void UpdateColliderSize(int width, int height)
        {
            this.box.w = width;
            this.box.h = height;
        }




        // Collision between two box colliders
        public override bool CollidesWith(BoxCollider other)
        {
            //boxcollider and boxcollider
            if (other is BoxCollider)
            {
                //check if the two boxes are colliding
                if (box.Intersects(other.box))
                {
                    Console.WriteLine("COLLISION!");
                   // Console.WriteLine("Box1: " + box.x + box.y);
                    return true;
                }
                return false;
            }
            return false;
        }
        
    }
    // defines a circle collider
    // a circle collider is a circle that can be used to detect collisions
    public class CircleCollider : Collider
    {
        private double radius = 1.0;
        public Vec2D center { get; set; }

        public CircleCollider(Vec2D center, double radius)
        {
            this.center = center;
            this.radius = radius;
        }
        public CircleCollider()
        {
            this.center = new Vec2D(0, 0);
            this.radius = 1.0;
        }

        // Collision between two circle colliders
        public override bool CollidesWith(CircleCollider other)
        {
            //circlecollider and circlecollider
            if (other is CircleCollider)
            {
                //check if the two circles are colliding
                if (DistanceTo(center, other.center) < radius + other.radius)
                {
                    //Console.WriteLine("COLLISION!");
                    return true;
                }
                return false;
            }
            return false;
        }

        //collision between circle and box
        public override bool CollidesWith(BoxCollider other)
        {
            //circlecollider and boxcollider
            if (other is BoxCollider)
            {
                // Calculate closest point to the circle's center within the box
                double closestX = Math.Max(other.box.x, Math.Min(center.x, other.box.x + other.box.w));
                double closestY = Math.Max(other.box.y, Math.Min(center.y, other.box.y + other.box.h));

                // Calculate distance between the circle's center and this closest point
                double distanceX = center.x - closestX;
                double distanceY = center.y - closestY;

                // If the distance is less than the circle's radius, they are colliding
                if ((distanceX * distanceX) + (distanceY * distanceY) < (radius * radius))
                {
                    Console.WriteLine("Circle + Box COLLISION!");
                    return true;
                }
                return false;
            }
            return false;
        }


        //create DistanceTo function
        public double DistanceTo(Vec2D center, Vec2D other)
        {
            return Math.Sqrt(Math.Pow(center.x - other.x, 2) + Math.Pow(center.y - other.y, 2));
        }
        //update the position of the circle collider
        internal void UpdateColliderPosition(Vec2D vec2D)
        {
           this.center = vec2D;
        }
        //update the size of the circle collider
        internal void UpdateColliderSize(double radius)
        {
            this.radius = radius;
        }
        
    }

    // defines an edge collider
    // an edge collider is a line segment that can be used to detect collisions
    public class  EdgeCollider : Collider
    {
        private Vec2D start = new Vec2D(0,0);
        private Vec2D end = new Vec2D(1, 1);

        
        public EdgeCollider()
        {
            this.start = new Vec2D(0, 0);
            this.end = new Vec2D(1, 1);
        }
        //update the position of the edge collider
        internal void UpdateColliderPosition(Vec2D start, Vec2D end)
        {
            this.start = start;
            this.end = end;
        }

        
        //collision between edgecollider and boxcollider
        public override bool CollidesWith(BoxCollider other)
        {
            //edgecollider and boxcollider
            if (other is BoxCollider)
            {
                //check if the box and edge are colliding
                if (Intersects(start, end, other.box))
                {
                    Console.WriteLine("Edge + Box COLLISION!");
                    return true;
                }


                return false;
            }
            return false;
        }

        private bool Intersects(Vec2D lineStart, Vec2D lineEnd, Rect box)
        {
            // Convert the box into four line segments
            Vec2D[] boxLines = new Vec2D[]
            {
                new Vec2D(box.x, box.y),
                new Vec2D(box.x + box.w, box.y),
                new Vec2D(box.x + box.w, box.y + box.h),
                new Vec2D(box.x, box.y + box.h)
            };

            // Check for intersection between the line segment and each line segment of the box
            for (int i = 0; i < 4; i++)
            {
                Vec2D current = boxLines[i];
                Vec2D next = boxLines[(i + 1) % 4];

                if (Intersects(lineStart, lineEnd, current, next))
                {
                    return true;
                }
            }

            return false;
        }

        // Helper method to check intersection between a line segment and another line segment
            private bool Intersects(Vec2D line1Start, Vec2D line1End, Vec2D line2Start, Vec2D line2End)
        {
            double q1 = CrossProduct(line2End - line2Start, line1Start - line2Start);
            double q2 = CrossProduct(line2End - line2Start, line1End - line2Start);
            double q3 = CrossProduct(line1End - line1Start, line2Start - line1Start);
            double q4 = CrossProduct(line1End - line1Start, line2End - line1Start);

            return q1 * q2 < 0 && q3 * q4 < 0;
        }

        // Helper method to compute the cross product of two vectors
        private double CrossProduct(Vec2D v1, Vec2D v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }
    }

    public class RaycastHit
    {
        // TODO: Implement RaycastHit class
        // This class should contain information about a collision between a ray and a collider
        private Collider? collider;
    }
    public static class Physics
    {
        public static bool Raycast(Vec2D origin, Vec2D direction, double distance, out RaycastHit hit)
        {
            // TODO: Implement Raycast method

            // Get active scene
            // Loop through all colliders in the scene
            // Check for collision between ray and collider
            // Return true if collision is found
            // Return false if no collision is found
            hit = new RaycastHit();
            return false;
        }

        // Adds gravity and other forces, moves objects
        public static void ApplePhysics(List<GameObject> gameObjects)
        { }

        // Checks for collisions between objects
        public static List<CollisionPair> CheckCollisions(List<GameObject> gameObjects)
        { 
            //check collisions
            var collisionPairList = new List<CollisionPair>();
            foreach (var gameObject1 in gameObjects)
            {
                if (gameObject1.GetComponent<Collider>() != null) 
                {                     
                    foreach (var gameObject2 in gameObjects)
                    {
                        if (gameObject2.GetComponent<Collider>() != null)
                        {
                            if (gameObject1 != gameObject2)
                            {
                                if (gameObject1.GetComponent<Collider>().CollidesWith(gameObject2.GetComponent<Collider>()))
                                {
                                    collisionPairList.Add(new CollisionPair(gameObject1, gameObject2));
                             
                                }
                            }
                        }
                    }
                }
            }
            return collisionPairList;
        }

        // Resolves collisions between objects
        // Moves objects apart so they are no longer colliding
        // Applies forces to objects after collision
        // For example, if two objects collide, they should bounce off each other based on their mass, velocity, bounciness, etc.
        public static void ResolveCollisions(List<CollisionPair> collisions)
        { }

        // after all collisions are resolved, notify objects that a collision has occured
        // TODO: Implement event listeners for OnCollisionEnter, OnCollisionStay, OnCollisionExit, etc.
        public static void NotifyCollisions(List<CollisionPair> collisions)
        { }


        /*
         * UpdatePhysics
         * Called every frame to update physics in several steps:
         * 1. Apply physics (velocity, gravity, forces, etc.)
         * 2. Check for collisions (filter out non-colliding objects, etc.)
         * 3. Resolve collisions (move objects apart, apply forces, etc.)
         * 4. Notify objects of collisions (call OnCollisionEnter, OnCollisionStay, OnCollisionExit, etc.) (EventListeners not implemented yet)
         * 
         * Some notes:
         * Not all objects need to have a physics body or collider
         * Objects without a physics body will not be affected by physics
         * Objects without a collider will not be checked for collisions
         * Objects with a PhysicsBody that has isMovable set to false will not be moved by physics
         * Object with a collider but without a physics body will not be moved by physics, but other moving objects can collide with them
         * to check if the object has a collider or physics body, use HasCollider() and HasPhysicsBody() methods
         * to get the collider or physics body of an object, use propterties collider and physicsBody or use generic GetComponent<T>() method
         */
        public static void UpdatePhysics(List<GameObject> gameObjects)
        {
            // Apply physics
            ApplePhysics(gameObjects);

            // Check for collisions
            List<CollisionPair> collisions = CheckCollisions(gameObjects);

            // Resolve collisions
            ResolveCollisions(collisions);

            // Notify objects of collisions
            NotifyCollisions(collisions);
        }

    }
}
