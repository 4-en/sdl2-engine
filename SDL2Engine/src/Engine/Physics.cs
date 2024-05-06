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
        private double mass = 1.0;
        private double bounciness = 1.0;
        private double friction = 0.0;
        private double drag = 0.0;

        public PhysicsBody()
        {
            this.isMovable = false;
            this.velocity = new Vec2D(0, 0);
            this.mass = 1.0;
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

        //get center of the collider
        public virtual Vec2D GetCenter()
        {
            return new Vec2D(0, 0);
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
        public Vec2D collisionPoint;

        public CollisionPair(GameObject obj1, GameObject obj2, Vec2D collisionPoint)
        {
            this.obj1 = obj1;
            this.obj2 = obj2;
            this.collisionPoint = collisionPoint;
        }
    }

    // defines a box collider
    // a box collider is a rectangle that can be used to detect collisions
    public class BoxCollider : Collider
    {

        //create a box collider at the position of the game object
        public Rect box { get; set; }

        //function to get the center position of the box collider
        public override Vec2D GetCenter()
        {
            return new Vec2D(box.x + box.w / 2, box.y + box.h / 2);
        }

        //get center
        public Vec2D GetCenter(BoxCollider boxCollider)
        {
            return new Vec2D(boxCollider.box.x + boxCollider.box.w / 2, boxCollider.box.y + boxCollider.box.h / 2);
        }


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
            var rect = new Rect(newPosition.x, newPosition.y, box.w, box.h);
            this.box = rect;
        }

        public void UpdateColliderSize(int width, int height)
        {
            var rect = new Rect(box.x, box.y, width, height);
            this.box = rect;
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
    public class EdgeCollider : Collider
    {
        private Vec2D start = new Vec2D(0, 0);
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
        public static void ApplyPhysics(List<GameObject> gameObjects)
        {
            // Apply gravity, forces, etc.
            foreach (var gameObject in gameObjects)
            {
                //apply gravity
                if (gameObject.GetComponent<PhysicsBody>() != null)
                {
                    if (gameObject.GetComponent<PhysicsBody>().IsMovable)
                    {
                        //gameObject.GetComponent<PhysicsBody>().Velocity = new Vec2D(gameObject.GetComponent<PhysicsBody>().Velocity.x, gameObject.GetComponent<PhysicsBody>().Velocity.y + 0.1);
                    }
                }
                //move objects
                if (gameObject.GetComponent<PhysicsBody>() != null)
                {
                    if (gameObject.GetComponent<PhysicsBody>().IsMovable)
                    {
                        gameObject.SetPosition(new Vec2D(gameObject.GetPosition().x + gameObject.GetComponent<PhysicsBody>().Velocity.x, gameObject.GetPosition().y + gameObject.GetComponent<PhysicsBody>().Velocity.y));
                        //move the collider with the object
                        if (gameObject.GetComponent<Collider>() != null)
                        {
                            if (gameObject.GetName().Equals("LeftPaddle") || gameObject.GetName().Equals("RightPaddle"))
                            {
                                (gameObject.GetComponent<Collider>() as BoxCollider).UpdateColliderPosition(new Vec2D(gameObject.GetPosition().x, (gameObject.GetPosition().y - 60)));

                            }
                            else if (gameObject.GetComponent<Collider>() is BoxCollider)
                            {
                                (gameObject.GetComponent<Collider>() as BoxCollider).UpdateColliderPosition(gameObject.GetPosition());

                            }
                            else if (gameObject.GetComponent<Collider>() is CircleCollider)
                            {
                                (gameObject.GetComponent<Collider>() as CircleCollider).UpdateColliderPosition(gameObject.GetPosition());
                            }
                            else if (gameObject.GetComponent<Collider>() is EdgeCollider)
                            {
                                (gameObject.GetComponent<Collider>() as EdgeCollider).UpdateColliderPosition(gameObject.GetPosition(), new Vec2D(gameObject.GetPosition().x + 50, gameObject.GetPosition().y + 50));
                            }
                        }
                    }
                }
            }
        }

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
                                    //check if collision pair already exists
                                    bool exists = false;
                                    foreach (var collisionPair in collisionPairList)
                                    {
                                        if (collisionPair.obj1 == gameObject2 && collisionPair.obj2 == gameObject1)
                                        {
                                            exists = true;
                                        }
                                    }
                                    if (!exists)
                                    {
                                        var cp = CalculateCollisionPoint(gameObject1, gameObject2);
                                        collisionPairList.Add(new CollisionPair(gameObject1, gameObject2, cp));
                                    }


                                }
                            }
                        }
                    }
                }
            }
            return collisionPairList;
        }

        private static Vec2D CalculateCollisionPoint(GameObject gameObject1, GameObject gameObject2)
        {
            //calculate the Collisionpoint between two objects
            //boxcollider and boxcollider
            if (gameObject1.GetComponent<Collider>() is BoxCollider && gameObject2.GetComponent<Collider>() is BoxCollider)
            {
                //calculate the collision point between two box colliders
                var box1 = gameObject1.GetComponent<Collider>() as BoxCollider;
                var box2 = gameObject2.GetComponent<Collider>() as BoxCollider;
                double x = Math.Max(box1.box.x, Math.Min(box2.box.x + box2.box.w, box1.box.x + box1.box.w));
                double y = Math.Max(box1.box.y, Math.Min(box2.box.y + box2.box.h, box1.box.y + box1.box.h));
                Vec2D collisionPoint = new Vec2D(x, y);
                Console.WriteLine("Collision Point: " + collisionPoint.x + collisionPoint.y);
                return collisionPoint;
            }
            //boxcollider and circlecollider
            if (gameObject1.GetComponent<Collider>() is BoxCollider && gameObject2.GetComponent<Collider>() is CircleCollider)
            {
                //calculate the collision point between a box collider and a circle collider
                var box = gameObject1.GetComponent<Collider>() as BoxCollider;
                var circle = gameObject2.GetComponent<Collider>() as CircleCollider;
                double closestX = Math.Max(box.box.x, Math.Min(circle.center.x, box.box.x + box.box.w));
                double closestY = Math.Max(box.box.y, Math.Min(circle.center.y, box.box.y + box.box.h));
                Vec2D collisionPoint = new Vec2D(closestX, closestY);
                Console.WriteLine("Collision Point: " + collisionPoint.x + collisionPoint.y);
                return collisionPoint;
            }
            //boxcollider and edgecollider
            if (gameObject1.GetComponent<Collider>() is BoxCollider && gameObject2.GetComponent<Collider>() is EdgeCollider)
            {
                //calculate the collision point between a box collider and an edge collider

                return new Vec2D(0, 0);
            }
            return new Vec2D(0, 0);
        }

        // Resolves collisions between objects
        // Moves objects apart so they are no longer colliding
        // Applies forces to objects after collision
        // For example, if two objects collide, they should bounce off each other based on their mass, velocity, bounciness, etc.
        public static void ResolveCollisions(List<CollisionPair> collisions)
        {
            foreach (var collision in collisions)
            {
                var obj1 = collision.obj1;
                var obj2 = collision.obj2;
                //check if the objects have a physics body
                if (obj1.GetComponent<PhysicsBody>() != null && obj2.GetComponent<PhysicsBody>() != null)
                {
                    //box collider and box collider
                    if (obj1.GetComponent<Collider>() is BoxCollider && obj2.GetComponent<Collider>() is BoxCollider)
                    {
                        //calculate the collision point between two box colliders
                        // var box1 = obj1.GetComponent<Collider>() as BoxCollider;
                        // var box2 = obj2.GetComponent<Collider>() as BoxCollider;


                        //Vec2D u1 = obj1.GetComponent<PhysicsBody>().Velocity;
                        //Vec2D u2 = obj2.GetComponent<PhysicsBody>().Velocity;

                        //Vec2D r1 = box1.GetCenter(box1) - collsion.collisionPoint;
                        //Vec2D r2 = box2.GetCenter(box2) - collsion.collisionPoint;
                        //Vec2D normal = r2 - r1;
                        // normal.Normalize();

                        //Vec2D u1n = normal * Vec2D.Dot(u1, normal);
                        //Vec2D u2n = normal * Vec2D.Dot(u2, normal);

                        //Vec2D v2n = (u1n * (obj1.GetComponent<PhysicsBody>().Mass - obj2.GetComponent<PhysicsBody>().Mass) + 2 * obj2.GetComponent<PhysicsBody>().Mass * u2n) / (obj1.GetComponent<PhysicsBody>().Mass + obj2.GetComponent<PhysicsBody>().Mass);
                        // Vec2D v1n = (u2n * (obj2.GetComponent<PhysicsBody>().Mass - obj1.GetComponent<PhysicsBody>().Mass) + 2 * obj1.GetComponent<PhysicsBody>().Mass * u1n) / (obj1.GetComponent<PhysicsBody>().Mass + obj2.GetComponent<PhysicsBody>().Mass);

                        //Vec2D v1 = (u1 - u1n) + v1n;
                        // Vec2D v2 = (u2 - u2n) + v2n;

                        // if (obj1.GetComponent<PhysicsBody>().IsMovable)
                        // {
                        //    obj1.GetComponent<PhysicsBody>().Velocity = v1;
                        //}
                        // if (obj2.GetComponent<PhysicsBody>().IsMovable)
                        //{
                        //   obj2.GetComponent<PhysicsBody>().Velocity = v2;
                        //}
                        // Console.WriteLine("v2: " + obj1.GetComponent<PhysicsBody>().Velocity.x + obj1.GetComponent<PhysicsBody>().Velocity.y);
                        // Console.WriteLine("v2: " + obj2.GetComponent<PhysicsBody>().Velocity.x + obj2.GetComponent<PhysicsBody>().Velocity.y);

                        //resolve collision for pong game
                        ResolveCollisionsForPong(obj1, obj2);
                    }
                }

            }
        }

        private static void ResolveCollisionsForPong(GameObject obj1, GameObject obj2)
        {

            if (obj1.GetComponent<PhysicsBody>().IsMovable)
            {
                //get name of gameobject
                string name1 = obj1.GetName();
                string name2 = obj2.GetName();
                if (!name1.Equals("PongSquare"))
                {
                    GameObject temp = obj1;
                    obj1 = obj2;
                    obj2 = temp;
                    name2 = obj2.GetName();
                }
                Vec2D vel = obj1.GetComponent<PhysicsBody>().Velocity;
                if (name2 == "BoarderLeft")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-vel.x, vel.y);
                }
                else if (name2 == "BoarderTop")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(vel.x, -vel.y);
                }
                else if (name2 == "BoarderRight")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-vel.x, vel.y);
                }
                else if (name2 == "BoarderBottom")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(vel.x, -vel.y);
                }
                else if (name2 == "LeftPaddle")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-vel.x, vel.y);
                }
                else if (name2 == "RightPaddle")
                {
                    obj1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-vel.x, vel.y);
                }


            }
            Console.WriteLine("Collision between" + obj1.GetName() + " and " + obj2.GetName());
        }

        public static double CalculateDotProduct(Vec2D vec1, Vec2D vec2)
        {
            if (vec1.Length != vec2.Length)
            {
                throw new ArgumentException("Vectors must have the same length.");
            }

            double result = 0;



            return result;
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
            ResolveCollisions(collisions);

            // Notify objects of collisions
            NotifyCollisions(collisions);
        }

    }
}
