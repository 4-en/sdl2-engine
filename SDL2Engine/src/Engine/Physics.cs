using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private bool isMovable = true;
        private Vec2D velocity = new Vec2D();
        private double angularVelocity = 0.0;
        private double mass = 1.0;
        private double bounciness = 1.0;
        private double friction = 0.0;
        private double drag = 0.0;

        public PhysicsBody()
        {
            this.isMovable = true;
            this.velocity = new Vec2D(0, 0);
            this.mass = 1;
            this.bounciness = 1.0;
            this.friction = 0.0;
            this.drag = 0.0;
        }

        public PhysicsBody(bool isMovable, Vec2D velocity, double mass, double bounciness, double friction, double drag)
        {
            this.isMovable = isMovable;
            this.velocity = velocity;
            this.mass = mass;
            this.bounciness = bounciness;
            this.friction = friction;
            this.drag = drag;
        }

        public bool IsMovable
        {
            get { return isMovable; }
            set { isMovable = value; }
        }

        public Vec2D Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public double AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public double Bounciness
        {
            get { return bounciness; }
            set { bounciness = value; }
        }

        public double Friction
        {
            get { return friction; }
            set { friction = value; }
        }

        public double Drag
        {
            get { return drag; }
            set { drag = value; }
        }

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


        public void UpdateColliderSize(int width, int height)
        {
            var rect = new Rect(box.x, box.y, width, height);
            this.box = rect;
        }

        public Rect GetCollisionBox()
        {
            return box + gameObject.GetPosition();
        }


        // Collision between two box colliders
        public override bool CollidesWith(BoxCollider other)
        {
            var cbox = this.GetCollisionBox();

            //check if the two boxes are colliding
            if (cbox.Intersects(other.GetCollisionBox()))
            {
                //Console.WriteLine("COLLISION!");
                // Console.WriteLine("Box1: " + box.x + box.y);
                return true;
            }
            return false;

        }

        public override bool CollidesWith(CircleCollider other)
        {
            return other.CollidesWith(this);
        }

        public override bool CollidesWith(EdgeCollider other)
        {
            return other.CollidesWith(this);
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

        public Vec2D GetCollisionCenter()
        {
            return center + gameObject.GetPosition();
        }

        public void SetCenter(Vec2D center)
        {
            this.center = center;
        }

        // Collision between two circle colliders
        public override bool CollidesWith(CircleCollider other)
        {

            //check if the two circles are colliding
            if (DistanceTo(this.GetCollisionCenter(), other.GetCollisionCenter()) < radius + other.radius)
            {
                //Console.WriteLine("COLLISION!");
                return true;
            }
            return false;

        }

        //collision between circle and box
        public override bool CollidesWith(BoxCollider other)
        {

            // Calculate closest point to the circle's center within the box
            Vec2D circleCenter = this.GetCollisionCenter();
            Rect box = other.GetCollisionBox();
            double closestX = Math.Max(box.x, Math.Min(circleCenter.x, box.x + box.w));
            double closestY = Math.Max(box.y, Math.Min(circleCenter.y, box.y + box.h));
            
            // Calculate distance between the circle's center and this closest point
            double distanceX = circleCenter.x - closestX;
            double distanceY = circleCenter.y - closestY;


            // If the distance is less than the circle's radius, they are colliding
            if ((distanceX * distanceX) + (distanceY * distanceY) < (radius * radius))
            {
                //Console.WriteLine("Circle + Box COLLISION!");
                return true;
            }
            return false;

        }


        //create DistanceTo function
        public double DistanceTo(Vec2D center, Vec2D other)
        {
            return Math.Sqrt(Math.Pow(center.x - other.x, 2) + Math.Pow(center.y - other.y, 2));
        }

        //update the size of the circle collider
        public void UpdateColliderSize(double radius)
        {
            this.radius = radius;
        }

    }

    // defines an edge collider
    // an edge collider is a line segment that can be used to detect collisions
    public class EdgeCollider : Collider
    {
        private Vec2D start = new Vec2D(0, 0);
        private Vec2D end = new Vec2D(1, 1);


        public EdgeCollider()
        {
            this.start = new Vec2D(0, 0);
            this.end = new Vec2D(1, 1);
        }

        public Vec2D[] GetCornerPoints()
        {
            return [start + gameObject.GetPosition(), end + gameObject.GetPosition()];
        }

        public void SetStart(Vec2D start)
        {
            this.start = start;
        }

        public void SetEnd(Vec2D end)
        {
            this.end = end;
        }

        public void SetEdge(Vec2D start, Vec2D end)
        {
            this.start = start;
            this.end = end;
        }


        //collision between edgecollider and boxcollider
        public override bool CollidesWith(BoxCollider other)
        {
            Rect box = other.GetCollisionBox();
            Vec2D[] corners = GetCornerPoints();
            //check if the box and edge are colliding
            if (Intersects(corners[0], corners[1], box))
            {
                //Console.WriteLine("Edge + Box COLLISION!");
                return true;
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
        public static void ApplyPhysics(List<GameObject> gameObjects)
        {
            double deltaTime = Time.deltaTime;
            // Apply gravity, forces, etc.
            foreach (var gameObject in gameObjects)
            {
                //apply gravity
                PhysicsBody? physicsBody = gameObject.GetComponent<PhysicsBody>();
                if (physicsBody == null) continue;
                if (!physicsBody.IsMovable) continue;


                //gameObject.GetComponent<PhysicsBody>().Velocity = new Vec2D(gameObject.GetComponent<PhysicsBody>().Velocity.x, gameObject.GetComponent<PhysicsBody>().Velocity.y + 0.1);


                //move objects
                Transform t = gameObject.transform;
                t.Move(
                    physicsBody.Velocity.x * deltaTime,
                    physicsBody.Velocity.y * deltaTime
                    );

                // rotate objects
                t.rotation += physicsBody.AngularVelocity * deltaTime;
                var vel_magnitude_squared = physicsBody.Velocity.LengthSquared();
                // if velocity is below a certain threshold, set it to 0
                if (vel_magnitude_squared > 0 && vel_magnitude_squared < 1.0)
                {
                    physicsBody.Velocity = new Vec2D(0, 0);
                    vel_magnitude_squared = 0;
                }

                // Apply friction and drag

                // drag: a force that opposes the motion of an object
                
                // new_kinetic_energy = old_kinetic_energy - drag_force
                // old_kinetic_energy = 0.5 * mass * velocity^2
                // drag_force = drag_coefficient * velocity^2
                // 
                // new_velocity       = old_velocity - 
                // new_kinetic_energy = 0.5 * mass * velocity^2 - drag_coefficient * velocity^2 
                // new_kinetic_energy = 0.5 * mass * velocity^2 * (1 - drag_coefficient)
                //
                //

                // skip if drag is 0 or velocity is 0
                if (physicsBody.Drag == 0 || vel_magnitude_squared == 0) continue;
                // Calculate the magnitude of the velocity squared

                // Normalize the velocity vector to get the direction of the force
                // use precomputed magnitude to avoid recomputing it when using Vec2D.Normalize()
                Vec2D velocity_normalized = physicsBody.Velocity / Math.Sqrt(vel_magnitude_squared);

                // Compute drag force: F_drag = -0.5 * rho * v^2 * C_d * A * unit_vector(v)
                // assume rho* C_d * A = physicsBody.Drag
                Vec2D dragForce = velocity_normalized * (-0.5  * vel_magnitude_squared * physicsBody.Drag);

                // Update velocity by adding acceleration due to drag (F = ma -> a = F/m)
                physicsBody.Velocity += dragForce / physicsBody.Mass * deltaTime;



            }
        }

        // Checks for collisions between objects
        public static List<CollisionPair> CheckCollisions(List<GameObject> gameObjects)
        {
            //check collisions
            Collider? colliderI = null;
            Collider? colliderJ = null;
            var collisionPairList = new List<CollisionPair>();
            for (uint i = 0; i < gameObjects.Count; i++)
            {
                colliderI = gameObjects[(int)i].GetComponent<Collider>();
                if (colliderI == null) continue;

                for (uint j = i + 1; j < gameObjects.Count; j++)
                {
                    colliderJ = gameObjects[(int)j].GetComponent<Collider>();
                    if (colliderJ == null) continue;


                    if (colliderI.CollidesWith(colliderJ))
                    {
                        collisionPairList.Add(new CollisionPair(gameObjects[(int)i], gameObjects[(int)j]));

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
        {


        }

        private static void ApplyFrictionAndDrag(PhysicsBody physicsBody)
        {
            // Apply friction
            Vec2D frictionForce = -physicsBody.Velocity;
            frictionForce.Normalize();
            frictionForce *= Math.Min(physicsBody.Friction * physicsBody.Mass, physicsBody.Velocity.Length());
            physicsBody.Velocity += frictionForce;

            // Apply drag
            Vec2D dragForce = -physicsBody.Velocity;
            dragForce.Normalize();
            dragForce *= Math.Min(physicsBody.Drag * physicsBody.Mass, physicsBody.Velocity.LengthSquared());
            physicsBody.Velocity += dragForce;
        }

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
            ApplyPhysics(gameObjects);

            // Check for collisions
            List<CollisionPair> collisions = CheckCollisions(gameObjects);

            // Resolve collisions
            //ResolveCollisions(collisions);

            // Notify objects of collisions
            NotifyCollisions(collisions);
        }

    }
}
