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
            return false;
        }

        public virtual bool CollidesWith(BoxCollider other)
        {
            return false;
        }

        public virtual bool CollidesWith(CircleCollider other)
        {
            return false;
        }

        public virtual bool CollidesWith(EdgeCollider other)
        {
            return false;
        }
    }

    public class CollisionPair
    {
        public Collider colliderA;
        public Collider colliderB;

        public CollisionPair(Collider colliderA, Collider colliderB)
        {
            this.colliderA = colliderA;
            this.colliderB = colliderB;
        }
    }

    public class BoxCollider : Collider
    { }

    public class CircleCollider : Collider
    { }

    public class  EdgeCollider : Collider
    { }

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
            return new List<CollisionPair>();
        }

        // Resolves collisions between objects
        // Moves objects apart so they are no longer colliding
        // Applies forces to objects after collision
        // For example, if two objects collide, they should bounce off each other based on their mass, velocity, etc.
        public static void ResolveCollisions(List<CollisionPair> collisions)
        { }

        // after all collisions are resolved, notify objects that a collision has occured
        public static void NotifyCollisions(List<CollisionPair> collisions)
        { }

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
