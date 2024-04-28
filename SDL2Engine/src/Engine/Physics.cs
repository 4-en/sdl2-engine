using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{

    public interface IBounded
    {
        // Rect has x, y, w, h properties
        Rect GetBounds();
        
    }

    // A QuadTree is a data structure that is used to store objects in a 2D space
    // It is used to quickly find objects that are close to each other
    // and therefore can be used to optimize collision detection by avoiding checking collisions between objects that are far away from each other
    public class QuadTree<T> where T : IBounded
    {
        private const int MAX_CAPACITY = 4;
        private List<T> items = new List<T>();
        private Rect bounds;
        private QuadTree<T>[]? children = null;
        private bool isDivided = false;

        public QuadTree(Rect bounds)
        {
            this.bounds = bounds;
        }

        public bool Insert(T item)
        {
            Rect itemBounds = item.GetBounds();
            if (!bounds.Intersects(itemBounds))
            {
                return false;
            }

            if (items.Count < MAX_CAPACITY && !isDivided)
            {
                items.Add(item);
                return true;
            }

            if (!isDivided)
            {
                Subdivide();
            }

            // Try to insert the item into each child.
            if (children == null)
            {
                return false;
            }
            bool inserted = false;
            foreach (var child in children)
            {
                if (child.Insert(item))
                {
                    inserted = true;
                }
            }
            return inserted;
        }

        private void Subdivide()
        {
            double x = bounds.x;
            double y = bounds.y;
            double halfWidth = bounds.w / 2;
            double halfHeight = bounds.h / 2;

            children = new QuadTree<T>[4]
            {
            new QuadTree<T>(new Rect(x, y, halfWidth, halfHeight)),
            new QuadTree<T>(new Rect(x + halfWidth, y, halfWidth, halfHeight)),
            new QuadTree<T>(new Rect(x, y + halfHeight, halfWidth, halfHeight)),
            new QuadTree<T>(new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight))
            };
            isDivided = true;

            List<T> tempItems = new List<T>(items);
            items.Clear();
            foreach (T item in tempItems)
            {
                Insert(item);
            }
        }

        public List<T> Query(Rect range, List<T>? found = null)
        {
            if (found == null)
            {
                found = new List<T>();
            }

            if (!bounds.Intersects(range))
            {
                return found;
            }

            if (!isDivided)
            {
                foreach (T item in items)
                {
                    if (item.GetBounds().Intersects(range))
                    {
                        found.Add(item);
                    }
                }
            }
            else if (children != null)
            {
                foreach (var child in children)
                {
                    child.Query(range, found);
                }
            }

            return found;
        }
    }

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
        private Rect box = new Rect(0, 0, 1, 1);

        // Collision between two box colliders
        public override bool CollidesWith(BoxCollider other)
        {
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
            return new List<CollisionPair>();
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
