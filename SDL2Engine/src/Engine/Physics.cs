using System;
using System.Collections.Generic;
using System.Linq;
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
        private Rect box = new Rect(0,0,1,1);

       //constructor
       //public BoxCollider(int a, int b)
       // {
         //   box = new Rect(a, b, 1, 1);
       // }

         
     
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
    }

    // defines an edge collider
    // an edge collider is a line segment that can be used to detect collisions
    public class  EdgeCollider : Collider
    {
        private Vec2D start = new Vec2D();
        private Vec2D end = new Vec2D();
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
