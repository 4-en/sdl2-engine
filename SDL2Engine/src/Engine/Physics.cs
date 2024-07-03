using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

    /*
     * Used to store multiple QuadTrees in an infinite 2D space
     * Only areas that have objects in them will have a QuadTree
     */
    public class InfiniteQuadTree<T> where T : IBounded
    {
        private Dictionary<int, QuadTree<T>> quadTrees = new Dictionary<int, QuadTree<T>>();
        private const int quadTreeSize = 10000;

        private int GetQuadTreeHash(int x, int y, int quadTreeSize)
        {
            int xIndex = x / quadTreeSize;
            int yIndex = y / quadTreeSize;
            int primeMultiplier = 10007; // A large prime number
            return xIndex + yIndex * primeMultiplier;
        }

        public void Insert(T item)
        {
            Rect bounds = item.GetBounds();
            int x = (int)bounds.x;
            int y = (int)bounds.y;
            int hash = GetQuadTreeHash(x, y, quadTreeSize);
            if (!quadTrees.ContainsKey(hash))
            {
                quadTrees[hash] = new QuadTree<T>(new Rect(x, y, quadTreeSize, quadTreeSize));
            }
            quadTrees[hash].Insert(item);
        }

        public LinkedList<T> Query(Rect range)
        {
            LinkedList<T> found = new ();
            int x = (int)range.x;
            int y = (int)range.y;
            int hash = GetQuadTreeHash(x, y, quadTreeSize);
            if (quadTrees.ContainsKey(hash))
            {
                quadTrees[hash].Query(range, found);
            }
            return found;
        }

        public IEnumerator<LinkedList<T>> GetPossibleCollisions()
        {
            foreach (var quadTree in quadTrees.Values)
            {
                var possibleCollisions = quadTree.GetPossibleCollisions();
                while (possibleCollisions.MoveNext())
                {
                    yield return possibleCollisions.Current;
                }
            }
        }
    }

    // A QuadTree is a data structure that is used to store objects in a 2D space
    // It is used to quickly find objects that are close to each other
    // and therefore can be used to optimize collision detection by avoiding checking collisions between objects that are far away from each other
    public class QuadTree<T> where T : IBounded
    {
        private const int MAX_CAPACITY = 16;
        private int maxDepth = 8;
        private LinkedList<T> items = new LinkedList<T>();
        private Rect bounds;
        private QuadTree<T>[]? children = null;
        private bool isDivided = false;

        public QuadTree(Rect bounds, int maxDepth = 8)
        {
            this.bounds = bounds;
            this.maxDepth = maxDepth;
        }

        private bool IntersectsMultiple(Rect range)
        {
            double centerX = bounds.x + bounds.w / 2;
            double centerY = bounds.y + bounds.h / 2;

            return range.x < centerX && range.x + range.w > centerX || range.y < centerY && range.y + range.h > centerY;
        }

        public bool Insert(T item)
        {
            Rect itemBounds = item.GetBounds();
            if (!bounds.Intersects(itemBounds))
            {
                return false;
            }

            if ((items.Count < MAX_CAPACITY && !isDivided) || maxDepth <= 0 || IntersectsMultiple(itemBounds))
            {
                items.AddLast(item);
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

            foreach (var child in children)
            {
                if (child.Insert(item))
                {
                    return true;
                }
            }
            return false;
        }

        private void Subdivide()
        {
            double x = bounds.x;
            double y = bounds.y;
            double halfWidth = bounds.w / 2;
            double halfHeight = bounds.h / 2;

            children =
                [
                    new QuadTree<T>(new Rect(x, y, halfWidth, halfHeight), maxDepth - 1),
                    new QuadTree<T>(new Rect(x + halfWidth, y, halfWidth, halfHeight), maxDepth - 1),
                    new QuadTree<T>(new Rect(x, y + halfHeight, halfWidth, halfHeight), maxDepth - 1),
                    new QuadTree<T>(new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight), maxDepth - 1)
                ];
            isDivided = true;

            var oldItems = items;
            this.items = new LinkedList<T>();
            foreach (T item in oldItems)
            {
                Insert(item);
            }
        }

        public LinkedList<T> Query(Rect range, LinkedList<T>? found = null)
        {
            if (found == null)
            {
                found = new LinkedList<T>();
            }

            if (!bounds.Intersects(range))
            {
                return found;
            }

            if (items.Count > 0)
            {
                foreach (T item in items)
                {
                    if (item.GetBounds().Intersects(range))
                    {
                        found.AddLast(item);
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

        /*
         * Yield returns List<T> of Ts that are in the same QuadTree node
         * Use this to compare collisions between objects in the same QuadTree node
         */
        public IEnumerator<LinkedList<T>> GetPossibleCollisions()
        {
            if (items.Count > 0)
            {
                yield return new LinkedList<T>(items);
            }
            if (children != null)
            {
                foreach (var child in children)
                {
                    var possibleCollisions = child.GetPossibleCollisions();
                    while (possibleCollisions.MoveNext())
                    {
                        foreach(var item in this.items)
                        {
                            // possibleCollisions.Current.AddLast(item);
                        }
                        yield return possibleCollisions.Current;
                    }
                }
            }

        }
    }

    // defines physical properties of an object
    // for example, mass, velocity, friction, etc.
    public class PhysicsBody : Component
    {
        // if true, objects with this component can be moved when colliding with other objects
        [JsonProperty]
        private bool isMovable = true;
        [JsonProperty]
        private Vec2D velocity = new Vec2D();
        [JsonProperty]
        private double angularVelocity = 0.0;
        [JsonProperty]
        private double mass = 1.0;
        [JsonProperty]
        private double bounciness = 1.0;
        [JsonProperty]
        private double friction = 0.0;
        [JsonProperty]
        private double drag = 0.0;
        [JsonProperty]
        private bool rotateWithVelocity = false;
        [JsonProperty]
        private bool collideWithMovingObjects = true;
        [JsonProperty]
        private bool collideWithStaticObjects = true;

        public PhysicsBody()
        {
            this.isMovable = true;
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

        public bool CollideWithMovingObjects
        {
            get { return collideWithMovingObjects; }
            set { collideWithMovingObjects = value; }
        }

        public bool CollideWithStaticObjects
        {
            get { return collideWithStaticObjects; }
            set { collideWithStaticObjects = value; }
        }

        public bool RotateWithVelocity
        {
            get { return rotateWithVelocity; }
            set { rotateWithVelocity = value; }
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

        public void SetVelocity(double x, double y)
        {
            velocity = new Vec2D(x, y);
        }

        public void SetVelocity(Vec2D velocity)
        {
            this.velocity = velocity;
        }

        public void AddVelocity(Vec2D velocity)
        {
            this.velocity += velocity;
        }

        public double AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }

        public void SetAngularVelocity(double angularVelocity)
        {
            this.angularVelocity = angularVelocity;
        }

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public void SetMass(double mass)
        {
            this.mass = mass;
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
    public class Collider : Component, IBounded
    {

        [JsonProperty]
        private bool isTrigger = false;

        // this is used to keep track of the active collision list
        // before every collision check, the active_index is swapped
        // this is used to call the correct callback functions for collision events
        [JsonIgnore]
        private uint active_index = 0;
        [JsonIgnore]
        private List<Collider>[] prevCollisions = { new List<Collider>(), new List<Collider>() };

        public void SwapCollisions()
        {
            active_index = (active_index + 1) % 2;
            prevCollisions[active_index].Clear();
        }

        public List<Collider> GetCurrentCollisionList()
        {
            return prevCollisions[active_index];
        }

        public List<Collider> GetPreviousCollisionList()
        {
            return prevCollisions[(active_index + 1) % 2];
        }

        public bool IsTrigger
        {
            get { return isTrigger; }
            set { isTrigger = value; }
        }

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

        public virtual Rect GetBounds()
        {
            var pos = gameObject.GetPosition();
            return new Rect(pos.x, pos.y, 0, 0);
        }
    }

    // defines a pair of objects that are colliding so they can be resolved later
    public class CollisionPair
    {
        public GameObject obj1;
        public GameObject obj2;
        public Vec2D collisionPoint;
        private bool cancelled = false;

        public CollisionPair(GameObject obj1, GameObject obj2, Vec2D collisionPoint)
        {
            this.obj1 = obj1;
            this.obj2 = obj2;
            this.collisionPoint = collisionPoint;
        }

        public GameObject GetOther(GameObject obj)
        {
            if (obj == obj1)
            {
                return obj2;
            }
            return obj1;
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }
    }

    // defines a box collider
    // a box collider is a rectangle that can be used to detect collisions
    public class BoxCollider : Collider
    {
        public static BoxCollider FromDrawableRect(DrawableRect drawableRect)
        {
            Rect r = drawableRect.GetRect();

            var collider = drawableRect.GetGameObject().AddComponent<BoxCollider>();
            collider.box = r;
            return collider;

        }
        public static BoxCollider? FromDrawableRect(GameObject gameObject)
        {
            Drawable? drawable = gameObject.GetComponent<Drawable>();
            if (drawable == null) return null;

            if (drawable is DrawableRect drawableRect)
            {
                Rect r = drawableRect.GetRect();

                var collider = gameObject.AddComponent<BoxCollider>();
                if (collider == null) return null;
                collider.box = r;
                return collider;

            }

            return null;
        }

        //create a box collider at the position of the game object
        [JsonProperty]
        public Rect box { get; set; }

        //function to get the center position of the box collider
        public override Vec2D GetCenter()
        {
            var box = this.GetCollisionBox();
            return new Vec2D(box.x + box.w / 2, box.y + box.h / 2);
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
            this.box = new Rect(0, 0, 50, 50);
        }

        public void UpdateColliderSize(int width, int height)
        {
            var rect = new Rect(this.box.x, this.box.y, width, height);
            this.box = rect;
        }

        public Rect GetCollisionBox()
        {
            return box + gameObject.GetPosition();
        }

        public override Rect GetBounds()
        {
            return GetCollisionBox();
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
        [JsonProperty]
        private double radius = 1.0;
        [JsonProperty]
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

        public override Rect GetBounds()
        {
            var pos = gameObject.GetPosition();
            return new Rect(pos.x - radius, pos.y - radius, radius * 2, radius * 2);
        }

        public double GetRadius()
        {
            return radius;
        }

        public void SetRadius(double radius)
        {
            this.radius = radius;
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
        [JsonProperty]
        private Vec2D start = new Vec2D(0, 0);
        [JsonProperty]
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

        public override Rect GetBounds()
        {
            var pos = gameObject.GetPosition();
            return new Rect(Math.Min(start.x, end.x) + pos.x, Math.Min(start.y, end.y) + pos.y, Math.Abs(start.x - end.x), Math.Abs(start.y - end.y));
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

        //update the position of the edge collider
        internal void UpdateColliderPosition(Vec2D start, Vec2D end)
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

    public class WorldSettings
    {
        public double gravity = 0;
        public double drag = 0.0;
        public double wind = 0.0;
        public bool enablePhysics = true;
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
        public static void ApplyPhysics(List<GameObject> gameObjects, WorldSettings? worldSettings = null)
        {
            double deltaTime = Time.deltaTime;
            // Apply gravity, forces, etc.
            foreach (var gameObject in gameObjects)
            {

                PhysicsBody? physicsBody = gameObject.GetComponent<PhysicsBody>();
                if (physicsBody == null) continue;
                if (!physicsBody.IsMovable) continue;
                if (!physicsBody.IsEnabled()) continue;


                // apply gravity
                if (worldSettings != null)
                {
                    if (worldSettings.gravity != 0)
                    {
                        double acclY = worldSettings.gravity * deltaTime;
                        physicsBody.SetVelocity(physicsBody.Velocity.x, physicsBody.Velocity.y + acclY);
                    }
                }


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
                // disabled, since it cause problems with gravity
                if (vel_magnitude_squared > 0 && vel_magnitude_squared < 1.0 && false)
                {
                    physicsBody.Velocity = new Vec2D(0, 0);
                    vel_magnitude_squared = 0;
                }
                else
                {
                    if (physicsBody.RotateWithVelocity)
                    {
                        // assuming textures are facing up by default
                        t.rotation = Math.Atan2(physicsBody.Velocity.y, physicsBody.Velocity.x) * 180 / Math.PI + 90;
                    }
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
                Vec2D dragForce = velocity_normalized * (-0.5 * vel_magnitude_squared * physicsBody.Drag);

                // check if the drag force is greater than the velocity
                // if so, set the velocity to 0
                var drag_velocity = dragForce / physicsBody.Mass * deltaTime;
                if (drag_velocity.LengthSquared() > vel_magnitude_squared)
                {
                    physicsBody.Velocity = new Vec2D(0, 0);
                }
                else
                {
                    // Update velocity by adding acceleration due to drag (F = ma -> a = F/m)
                    physicsBody.Velocity += dragForce / physicsBody.Mass * deltaTime;
                }



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
                if (!colliderI.IsEnabled()) continue;


                // We can skip checking FROM objects that don't have a physics body
                // we also don't need to check for collisions with objects that meet the following conditions:
                // - they are not enabled
                // - the physics body is not movable
                // - the velocity is 0

                PhysicsBody? physicsBody = gameObjects[(int)i].GetComponent<PhysicsBody>();
                if (physicsBody == null || !physicsBody.IsMovable || !physicsBody.IsEnabled() || physicsBody.Velocity.LengthSquared() == 0) continue;

                colliderI.SwapCollisions();


                for (uint j = 0; j < gameObjects.Count; j++)
                {
                    colliderJ = gameObjects[(int)j].GetComponent<Collider>();
                    if (colliderJ == null) continue;
                    if (!colliderJ.IsEnabled()) continue;

                    var physicsBodyJ = gameObjects[(int)j].GetComponent<PhysicsBody>();
                    if (physicsBodyJ != null && j <= i) continue;


                    if (physicsBody.CollideWithMovingObjects && physicsBody.CollideWithStaticObjects)
                    {
                        colliderJ.SwapCollisions();

                        if (colliderI.CollidesWith(colliderJ))
                        {
                            var cp = CalculateCollisionPoint(gameObjects[(int)i], gameObjects[(int)j]);
                            collisionPairList.Add(new CollisionPair(gameObjects[(int)i], gameObjects[(int)j], cp));
                            colliderJ.GetCurrentCollisionList().Add(colliderI);
                            colliderI.GetCurrentCollisionList().Add(colliderJ);

                        }
                    }
                    else if (physicsBody.CollideWithStaticObjects)
                    {
                        if (physicsBodyJ != null && physicsBodyJ.IsMovable) continue;

                        colliderJ.SwapCollisions();

                        if (colliderI.CollidesWith(colliderJ))
                        {
                            var cp = CalculateCollisionPoint(gameObjects[(int)i], gameObjects[(int)j]);
                            collisionPairList.Add(new CollisionPair(gameObjects[(int)i], gameObjects[(int)j], cp));
                            colliderJ.GetCurrentCollisionList().Add(colliderI);
                            colliderI.GetCurrentCollisionList().Add(colliderJ);

                        }
                    }
                    else if (physicsBody.CollideWithMovingObjects)
                    {
                        if (physicsBodyJ == null || !physicsBodyJ.IsMovable) continue;

                        colliderJ.SwapCollisions();

                        if (colliderI.CollidesWith(colliderJ))
                        {
                            var cp = CalculateCollisionPoint(gameObjects[(int)i], gameObjects[(int)j]);
                            collisionPairList.Add(new CollisionPair(gameObjects[(int)i], gameObjects[(int)j], cp));
                            colliderJ.GetCurrentCollisionList().Add(colliderI);
                            colliderI.GetCurrentCollisionList().Add(colliderJ);

                        }
                    }


                }

            }
            return collisionPairList;
        }

        // TODO: fix this function
        // this should probably be a method of the Collider class
        // and does not have to be seperate from the CheckCollisions method
        // otherwise we are doing the same calculations twice
        private static Vec2D CalculateCollisionPoint(GameObject gameObject1, GameObject gameObject2)
        {
            //calculate the Collisionpoint between two objects
            //boxcollider and boxcollider
            if (gameObject1.GetComponent<Collider>() is BoxCollider && gameObject2.GetComponent<Collider>() is BoxCollider)
            {
                //calculate the collision point between two box colliders
                var box1 = gameObject1.GetComponent<Collider>() as BoxCollider;
                var box2 = gameObject2.GetComponent<Collider>() as BoxCollider;
                if (box1 == null || box2 == null) return new Vec2D(0, 0);
                double x = Math.Max(box1.box.x, Math.Min(box2.box.x + box2.box.w, box1.box.x + box1.box.w));
                double y = Math.Max(box1.box.y, Math.Min(box2.box.y + box2.box.h, box1.box.y + box1.box.h));
                Vec2D collisionPoint = new Vec2D(x, y);
                // Console.WriteLine("Collision Point: " + collisionPoint.x + collisionPoint.y);
                return collisionPoint;
            }
            //boxcollider and circlecollider
            if (gameObject1.GetComponent<Collider>() is BoxCollider && gameObject2.GetComponent<Collider>() is CircleCollider)
            {
                //calculate the collision point between a box collider and a circle collider
                var box = gameObject1.GetComponent<Collider>() as BoxCollider;
                var circle = gameObject2.GetComponent<Collider>() as CircleCollider;
                if (box == null || circle == null) return new Vec2D(0, 0);
                double closestX = Math.Max(box.box.x, Math.Min(circle.center.x, box.box.x + box.box.w));
                double closestY = Math.Max(box.box.y, Math.Min(circle.center.y, box.box.y + box.box.h));
                Vec2D collisionPoint = new Vec2D(closestX, closestY);
                // Console.WriteLine("Collision Point: " + collisionPoint.x + collisionPoint.y);
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
        public static void ResolveCollisionsOld(List<CollisionPair> collisions)
        {
            foreach (var collision in collisions)
            {
                if (collision.IsCancelled()) continue;

                var obj1 = collision.obj1;
                var obj2 = collision.obj2;
                var collisionPoint = collision.collisionPoint;

                var pb1 = obj1.GetComponent<PhysicsBody>();
                var pb2 = obj2.GetComponent<PhysicsBody>();
                var box1 = obj1.GetComponent<BoxCollider>();
                var box2 = obj2.GetComponent<BoxCollider>();

                // check for nulls
                if (pb1 == null || box1 == null || box2 == null) continue;

                // check if any of the colliders is a trigger
                // if so, do not resolve the collision
                // triggers are used to detect collisions without resolving them
                // they are still used to notify objects of collisions, so they can react to them
                // useful for example for detecting when a player enters a trigger area
                // or when implementing custom collision behaviour
                if (box1.IsTrigger || box2.IsTrigger) continue;

                // calculate the direction of the collision
                // this is either horizontal or vertical, since we are only dealing with boxes
                // for now, just check if box1 center is to the left or right of box2.
                // If so, the collision is horizontal, otherwise it is vertical
                Vec2D collisionNormal = new Vec2D(0, 0);
                Vec2D box1Center = box1.GetCenter();
                Rect box2Area = box2.GetCollisionBox();
                if (box1Center.x < box2Area.x)
                {
                    collisionNormal = new Vec2D(-1, 0);
                }
                else if (box1Center.x > box2Area.x + box2Area.w)
                {
                    collisionNormal = new Vec2D(1, 0);
                }
                else if (box1Center.y < box2Area.y)
                {
                    collisionNormal = new Vec2D(0, -1);
                }
                else if (box1Center.y > box2Area.y + box2Area.h)
                {
                    collisionNormal = new Vec2D(0, 1);
                }

                Vec2D relativeVelocity = pb1.Velocity;
                double velocityAlongNormal = Vec2D.Dot(relativeVelocity, collisionNormal);

                // check if the objects are moving away from each other
                if (velocityAlongNormal > 0)
                {
                    continue;
                }

                // calculate the impulse scalar
                double e = Math.Min(pb1.Bounciness, 1);
                double j = -(1 + e) * velocityAlongNormal;
                j /= 1 / pb1.Mass;

                // apply the impulse
                Vec2D impulse = collisionNormal * j;
                pb1.Velocity += impulse;

                // just to make sure the objects are not stuck together, move obj1 away from obj2 by a small amount
                double dt = Time.deltaTime / 2;
                obj1.transform.Move(impulse.x * dt, impulse.y * dt);

            }
        }

        // Resolves collisions between objects
        // Moves objects apart so they are no longer colliding
        // Applies forces to objects after collision
        // For example, if two objects collide, they should bounce off each other based on their mass, velocity, bounciness, etc.
        public static void ResolveCollisions(List<CollisionPair> collisions)
        {

            var getOverlap = (Rect rect1, Rect rect2) =>
            {
                double x_overlap = Math.Min(rect1.x + rect1.w, rect2.x + rect2.w) - Math.Max(rect1.x, rect2.x);
                double y_overlap = Math.Min(rect1.y + rect1.h, rect2.y + rect2.h) - Math.Max(rect1.y, rect2.y);

                return new Vec2D(x_overlap, y_overlap);
            };

            var getNormal = (Rect rect1, Rect rect2, Vec2D overlap) =>
            {
                Vec2D normal = new Vec2D(0, 0);

                if (overlap.x < overlap.y)
                {
                    if(rect1.x < rect2.x)
                    {
                        normal = new Vec2D(-1, 0);
                    }
                    else
                    {
                        normal = new Vec2D(1, 0);
                    }
                }
                else
                {
                    if (rect1.y < rect2.y)
                    {
                        normal = new Vec2D(0, -1);
                    }
                    else
                    {
                        normal = new Vec2D(0, 1);
                    }
                }

                return normal;
            };

            var seperate = (Rect rect1, GameObject go1, Rect rect2, GameObject go2, Vec2D normal, Vec2D overlap) =>
            {
                PhysicsBody? pb1 = go1.GetComponent<PhysicsBody>();
                PhysicsBody? pb2 = go2.GetComponent<PhysicsBody>();

                bool move1 = pb1 != null && pb1.IsMovable;
                bool move2 = pb2 != null && pb2.IsMovable;

                double friction1 = pb1 != null ? pb1.Friction : 0;
                double friction2 = pb2 != null ? pb2.Friction : 0;

                double combinedFriction = (friction1 + friction2) * Time.deltaTime;

                bool eitherNonMovable = !move1 || !move2;
                double halfMovement = eitherNonMovable ? 1 : 0.5;
                if (normal.x != 0)
                {
                    double move_x = overlap.x * normal.x;

                    if (pb1 != null && move1)
                    {
                        pb1.Velocity = new Vec2D(pb1.Velocity.x * -pb1.Bounciness, pb1.Velocity.y * Math.Max(0,1 - combinedFriction));
                        go1.transform.Move(move_x * halfMovement, 0);
                    }

                    if (pb2 != null && move2)
                    {
                        pb2.Velocity = new Vec2D(pb2.Velocity.x * -pb2.Bounciness, pb2.Velocity.y * Math.Max(0, 1 - combinedFriction));
                        go2.transform.Move(-move_x * halfMovement, 0);
                    }
                }
                else
                {
                    double move_y = overlap.y * normal.y;
                    
                    if(pb1 != null && move1)
                    {
                        pb1.Velocity = new Vec2D(pb1.Velocity.x * Math.Max(0, 1 - combinedFriction), pb1.Velocity.y * -pb1.Bounciness);
                        go1.transform.Move(0, move_y * halfMovement);
                    }
                    if(pb2 != null && move2)
                    {
                        pb2.Velocity = new Vec2D(pb2.Velocity.x * Math.Max(0, 1 - combinedFriction), pb2.Velocity.y * -pb2.Bounciness);
                        go2.transform.Move(0, -move_y * halfMovement);
                    }
                }
            };

            // Console.WriteLine("Resolving collisions: " + collisions.Count);

            foreach (var collision in collisions)
            {
                if (collision.IsCancelled()) continue;

                var obj1 = collision.obj1;
                var obj2 = collision.obj2;
                var collisionPoint = collision.collisionPoint;

                var pb1 = obj1.GetComponent<PhysicsBody>();
                var pb2 = obj2.GetComponent<PhysicsBody>();
                var box1 = obj1.GetComponent<BoxCollider>();
                var box2 = obj2.GetComponent<BoxCollider>();

                // check for nulls
                if (box1 == null || box2 == null) continue;

                // check if any of the colliders is a trigger
                // if so, do not resolve the collision
                // triggers are used to detect collisions without resolving them
                // they are still used to notify objects of collisions, so they can react to them
                // useful for example for detecting when a player enters a trigger area
                // or when implementing custom collision behaviour
                if (box1.IsTrigger || box2.IsTrigger) continue;

                // calculate the direction of the collision
                // this is either horizontal or vertical, since we are only dealing with boxes
                // for now, just check if box1 center is to the left or right of box2.
                // If so, the collision is horizontal, otherwise it is vertical


                var overlap = getOverlap(box1.GetCollisionBox(), box2.GetCollisionBox());
                var normal = getNormal(box1.GetCollisionBox(), box2.GetCollisionBox(), overlap);

                seperate(box1.GetCollisionBox(), obj1, box2.GetCollisionBox(), obj2, normal, overlap);


                /*
                Vec2D collisionNormal = new Vec2D(0, 0);
                Vec2D box1Center = box1.GetCenter();
                Rect box2Area = box2.GetCollisionBox();
                if (box1Center.x < box2Area.x)
                {
                    collisionNormal = new Vec2D(-1, 0);
                }
                else if (box1Center.x > box2Area.x + box2Area.w)
                {
                    collisionNormal = new Vec2D(1, 0);
                }
                else if (box1Center.y < box2Area.y)
                {
                    collisionNormal = new Vec2D(0, -1);
                }
                else if (box1Center.y > box2Area.y + box2Area.h)
                {
                    collisionNormal = new Vec2D(0, 1);
                }

                if (pb1 != null)
                {
                    Vec2D relativeVelocity1 = pb1.Velocity;
                    double velocityAlongNormal1 = Vec2D.Dot(relativeVelocity1, collisionNormal);

                    // calculate the impulse scalar
                    double e = Math.Max(pb1.Bounciness, 0);
                    double j = -(1 + e) * velocityAlongNormal1;
                    j /= 1 / pb1.Mass;

                    // apply the impulse
                    Vec2D impulse = collisionNormal * j;
                    pb1.Velocity += impulse;

                    // just to make sure the objects are not stuck together, move obj1 away from obj2 by a small amount
                    //double dt = Time.deltaTime / 2;
                    //obj1.transform.Move(impulse.x * dt, impulse.y * dt);
                }


                if (pb2 != null)
                {

                    collisionNormal = -collisionNormal;

                    Vec2D relativeVelocity2 = pb2.Velocity;
                    double velocityAlongNormal2 = Vec2D.Dot(relativeVelocity2, collisionNormal);

                    // calculate the impulse scalar
                    double e = Math.Max(pb2.Bounciness, 0);
                    double j = -(1 + e) * velocityAlongNormal2;
                    j /= 1 / pb2.Mass;

                    // apply the impulse
                    Vec2D impulse = collisionNormal * j;
                    pb2.Velocity += impulse;

                    // just to make sure the objects are not stuck together, move obj1 away from obj2 by a small amount
                    //double dt = Time.deltaTime / 2;
                    //obj2.transform.Move(impulse.x * dt, impulse.y * dt);
                }




                // check if the objects are moving away from each other
                // if (velocityAlongNormal1 > 0)
                // {
                //    continue;
                // }
                */
            }
        }

        private static void NotifyPreResolveCollisions(List<CollisionPair> collisions)
        {
            foreach (var pair in collisions)
            {
                GameObject obj1 = pair.obj1;
                GameObject obj2 = pair.obj2;

                Collider? collider1 = obj1.GetComponent<Collider>();
                Collider? collider2 = obj2.GetComponent<Collider>();

                if (collider1 == null || collider2 == null) continue;

                List<Script> obj1Scripts = obj1.GetComponents<Script>();
                List<Script> obj2Scripts = obj2.GetComponents<Script>();

                foreach (var script in obj1Scripts)
                {
                    script.OnPreCollision(pair);
                }
                foreach (var script in obj2Scripts)
                {
                    script.OnPreCollision(pair);
                }
            }
        }


        // after all collisions are resolved, notify objects that a collision has occured
        public static void NotifyCollisions(List<CollisionPair> collisions)
        {
            foreach (var pair in collisions)
            {
                GameObject obj1 = pair.obj1;
                GameObject obj2 = pair.obj2;

                Collider? collider1 = obj1.GetComponent<Collider>();
                Collider? collider2 = obj2.GetComponent<Collider>();

                if (collider1 == null || collider2 == null) continue;

                bool wasPreviouslyColliding = collider1.GetPreviousCollisionList().Contains(collider2);

                List<Script> obj1Scripts = obj1.GetComponents<Script>();
                List<Script> obj2Scripts = obj2.GetComponents<Script>();

                if (wasPreviouslyColliding)
                {
                    foreach (var script in obj1Scripts)
                    {
                        script.OnCollisionStay(pair);
                    }
                    foreach (var script in obj2Scripts)
                    {
                        script.OnCollisionStay(pair);
                    }
                }
                else
                {
                    foreach (var script in obj1Scripts)
                    {
                        script.OnCollisionEnter(pair);
                    }
                    foreach (var script in obj2Scripts)
                    {
                        script.OnCollisionEnter(pair);
                    }
                }


            }
        }


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
        public static void UpdatePhysics(List<GameObject> gameObjects, Rect? bounds = null, WorldSettings? worldSettings = null)
        {

            if (bounds != null)
            {
                // TODO: fix QuadTree and use optimized physics update
                //UpdatePhysicsInArea(gameObjects, bounds.Value, worldSettings);
                //return;

                List<GameObject> objectsInBounds = new List<GameObject>();

                foreach (var obj in gameObjects)
                {
                    if (bounds.Value.Contains(obj.GetPosition()))
                    {
                        objectsInBounds.Add(obj);
                    }
                }

                gameObjects = objectsInBounds;

            }

            if (worldSettings == null || worldSettings.enablePhysics == true)
            {
                // Apply physics
                ApplyPhysics(gameObjects, worldSettings);
            }

            // Check for collisions
            List<CollisionPair> collisions = CheckCollisions(gameObjects);

            // Notify objects before resolving collisions
            NotifyPreResolveCollisions(collisions);

            // Resolve collisions
            ResolveCollisions(collisions);

            // Notify objects of collisions
            NotifyCollisions(collisions);
        }


        private static void UpdatePhysicsInArea(List<GameObject> gameObjects, Rect bounds, WorldSettings? worldSettings = null)
        {
            List<GameObject> objectsInBounds = new List<GameObject>();

            foreach (var obj in gameObjects)
            {
                if (bounds.Contains(obj.GetPosition()))
                {
                    objectsInBounds.Add(obj);
                }
            }

            if (worldSettings == null || worldSettings.enablePhysics == true)
            {
                // Apply physics
                ApplyPhysics(objectsInBounds, worldSettings);
            }

            // seperate using quadtree
            QuadTree<Collider> quadTree = new QuadTree<Collider>(bounds);
            foreach (var obj in objectsInBounds)
            {
                Collider? collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    quadTree.Insert(collider);
                }
            }

            // Check for collisions
            List<CollisionPair> collisions = new List<CollisionPair>();
            var possibleCollisions = quadTree.GetPossibleCollisions();
            while (possibleCollisions.MoveNext())
            {
                var colliders = possibleCollisions.Current;
                var gameObjectsInQuadTree = colliders.Select(collider => collider.GetGameObject()).ToList();

                List<CollisionPair> quadPairs = CheckCollisions(gameObjectsInQuadTree);
                if (quadPairs.Count > 0)
                {
                    collisions.AddRange(quadPairs);
                }
            }

            // Notify objects before resolving collisions
            NotifyPreResolveCollisions(collisions);

            // Resolve collisions
            ResolveCollisions(collisions);

            // Notify objects of collisions
            NotifyCollisions(collisions);


        }

        [StructLayout(LayoutKind.Sequential)]
        struct _ObjectRef
        {
            public int object_index;
            public float x;
            public float y;
            public float w;
            public float h;
        }
        /*
        [DllImport("SDL2EngineNative", CallingConvention = CallingConvention.Cdecl)]
        extern static private unsafe IntPtr _GroupGameObjects(_ObjectRef[] objects, int count, float x, float y, float w, float h);

        /*
         * _UnsafePhysicsInArea
         * Optimized function using c code to gather all objects in the area
         * TODO: implement this function
         
        public static void _UnsafePhysicsInArea(List<GameObject> gameObjects, Rect area)
        {
            float x = (float)area.x;
            float y = (float)area.y;
            float w = (float)area.w;
            float h = (float)area.h;

            LinkedList<GameObject> noColliderButInBounds = new LinkedList<GameObject>();
            _ObjectRef[] objects = new _ObjectRef[gameObjects.Count];
            int trueCount = 0;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (!area.Contains(gameObjects[i].GetPosition()))
                {
                    continue;
                }

                Collider? collider = gameObjects[i].GetComponent<Collider>();
                if (collider == null)
                {
                    noColliderButInBounds.AddLast(gameObjects[i]);
                    continue;
                }

                Rect bounds = collider.GetBounds();
                objects[trueCount] = new _ObjectRef
                {
                    object_index = i,
                    x = (float)bounds.x,
                    y = (float)bounds.y,
                    w = (float)bounds.w,
                    h = (float)bounds.h
                };
                trueCount++;
                
            }

            LinkedList<GameObject[]> collisionAreas = new LinkedList<GameObject[]>();

            unsafe
            {
                IntPtr ptr = _GroupGameObjects(objects, trueCount, x, y, w, h);

                /*
                 * IntPtr is a pointer to a memory location of arrays of integers
                 * The first integer is the number of arrays
                 * Each of the arrays contains the indices of the objects in the area
                 * Their first element is the number of objects in the array
                 

                int* intPtr = (int*)ptr.ToPointer();

                /*
                 * ...
                 * foreach array in the pointer
                 *   GameObject[] objects = new GameObject[array[0]]
                 *   for i = 0 to array[0]
                 *     objects[i] = gameObjects[array[i + 1]]
                 *     
                 *   Marshal.FreeHGlobal(array)
                 

                Marshal.FreeHGlobal(ptr);
            }

            ApplyPhysics(noColliderButInBounds.ToList());

            foreach(GameObject[] collArea in collisionAreas)
            {
                ApplyPhysics([.. collArea]);
            }

            LinkedList<CollisionPair> collisionPairs = new LinkedList<CollisionPair>();
            foreach (GameObject[] collArea in collisionAreas)
            {
                var pairs = CheckCollisions([.. collArea]);
                foreach (var pair in pairs)
                {
                    collisionPairs.AddLast(pair);
                }
            }

            NotifyPreResolveCollisions(collisionPairs.ToList());

            ResolveCollisions(collisionPairs.ToList());

            NotifyCollisions(collisionPairs.ToList());


            

        }
        */
    }
}
