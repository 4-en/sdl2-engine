using SDL2;
using static SDL2.SDL;
using System;

namespace SDL2Engine
{

    // Used to calculate origin relative to GameObject position
    public enum AnchorPoint
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    struct Color
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    // DepthBuffer class to store depth values for each pixel of the screen
    // Can be used for occlusion culling
    public class DepthBuffer
    {
        private float[,] buffer = new float[Engine.windowWidth, Engine.windowHeight];
        private float minDepth = 0;
        private float maxDepth = 1;

        // TODO: Implement DepthBuffer class
        // maybe this is not necessary for now, but keep in mind if performance is an issue
    }

    public abstract class Camera : Component
    {
        public abstract Vec2D WorldToScreen(Vec2D worldPosition, Vec2D rootPosition = new Vec2D());
        public abstract Vec2D GetScreenSize();
        public abstract Vec2D ScreenToWorld(Vec2D screenPosition);

        public abstract Vec2D GetWorldSize();

        public static Camera? GetCamera(GameObject gameObject)
        {
            var root = gameObject.GetScene();
            if (root != null)
            {
                Scene scene = root;
                return scene.GetCamera();
            }
            return null;
        }

        public abstract Rect RectToScreen(Rect rect, Vec2D rootPosition = new Vec2D());
        public abstract Rect RectToWorld(Rect rect, Vec2D rootPosition = new Vec2D());
    }

    public class Camera2D : Camera
    {
        private Vec2D Position { get; set; }
        private Vec2D Size { get; set; }

        public bool keepAspectRatio = true;


        public Camera2D(Vec2D position = new Vec2D())
        {
            this.Position = position;
            this.Size = new Vec2D(1920, 1080);
        }

        public override Vec2D GetWorldSize()
        {
            return Size;
        }

        public override Vec2D GetScreenSize()
        {
            if (keepAspectRatio)
            {
                double aspectRatio = Size.x / Size.y;
                double windowAspectRatio = Engine.windowWidth / Engine.windowHeight;

                // constant scale for both axes
                if (aspectRatio > windowAspectRatio)
                {
                    return new Vec2D(Engine.windowWidth, Engine.windowWidth / aspectRatio);
                }
                else
                {
                    return new Vec2D(Engine.windowHeight * aspectRatio, Engine.windowHeight);
                }
            }

            return new Vec2D(Engine.windowWidth, Engine.windowHeight);
        }

        public override Vec2D ScreenToWorld(Vec2D screenPosition)
        {
            Vec2D screenSize = GetScreenSize();
            return new Vec2D((screenPosition.x - Position.x) / screenSize.x * Size.x, (screenPosition.y - Position.y) / screenSize.y * Size.y);
        }

        public override Vec2D WorldToScreen(Vec2D worldPosition, Vec2D rootPosition = new Vec2D())
        {
            Vec2D screenSize = GetScreenSize();
            return new Vec2D((worldPosition.x + rootPosition.x) / Size.x * screenSize.x + Position.x, (worldPosition.y + rootPosition.y) / Size.y * screenSize.y + Position.y);
        }

        public override Rect RectToScreen(Rect rect, Vec2D rootPosition = new Vec2D())
        {
            Vec2D topLeft = WorldToScreen(rect.GetTopLeft(), rootPosition);
            Vec2D bottomRight = WorldToScreen(rect.GetBottomRight(), rootPosition);
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }

        public override Rect RectToWorld(Rect rect, Vec2D rootPosition = new Vec2D())
        {
            Vec2D topLeft = ScreenToWorld(rect.GetTopLeft());
            Vec2D bottomRight = ScreenToWorld(rect.GetBottomRight());
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }

    }

    // Base class for all drawable components
    // these components are called by their GameObjects to draw themselves
    public class Drawable : Component
    {

        protected AnchorPoint anchorPoint = AnchorPoint.Center;

        public virtual void Draw(Camera camera)
        {
            throw new NotImplementedException();
        }

        public virtual Vec2D GetDrawRoot()
        {
            var goRoot = this.gameObject.GetPosition();

            // for implemented Drawables, we should use the outline of the drawable to calculate the root with the anchor point
            // in this case, we ignore the anchor point and just return the position of the GameObject

            Camera? camera = GetCamera();
            if (camera != null)
            {
                return camera.WorldToScreen(goRoot);
            }

            return goRoot;

        }
    }

    public class DrawableRect : Drawable
    {
        protected Rect rect = new Rect(0, 0, 100, 100);

        public SDL.SDL_Rect GetDestRect()
        {
            return this.scene?.GetCamera().RectToScreen(rect, this.gameObject.GetPosition()).ToSDLRect() ?? rect.ToSDLRect();
        }

        public override void Draw(Camera camera)
        {
            var renderer = Engine.renderer;

            Vec2D root = GetDrawRoot();
            Vec2D size = rect.GetSize();

            Vec2D topLeft = new Vec2D(root.x - size.x / 2, root.y - size.y / 2);
            Vec2D topRight = new Vec2D(root.x + size.x / 2, root.y - size.y / 2);
            Vec2D bottomLeft = new Vec2D(root.x - size.x / 2, root.y + size.y / 2);
            Vec2D bottomRight = new Vec2D(root.x + size.x / 2, root.y + size.y / 2);

            topLeft = camera.WorldToScreen(topLeft);
            topRight = camera.WorldToScreen(topRight);
            bottomLeft = camera.WorldToScreen(bottomLeft);
            bottomRight = camera.WorldToScreen(bottomRight);

            SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL_RenderDrawLine(renderer, (int)topLeft.x, (int)topLeft.y, (int)topRight.x, (int)topRight.y);
            SDL_RenderDrawLine(renderer, (int)topRight.x, (int)topRight.y, (int)bottomRight.x, (int)bottomRight.y);
            SDL_RenderDrawLine(renderer, (int)bottomRight.x, (int)bottomRight.y, (int)bottomLeft.x, (int)bottomLeft.y);
            SDL_RenderDrawLine(renderer, (int)bottomLeft.x, (int)bottomLeft.y, (int)topLeft.x, (int)topLeft.y);
        }

    }

    public class Texture : DrawableRect
    {
        private IntPtr texture = IntPtr.Zero;
        private string? path = null;
        public static readonly string rootTexturePath = "Assets/Textures/";

        ~Texture()
        {
            // TODO: check if this actually works
            // seems like the finalizer is not called
            Console.WriteLine("Texture destroyed");
            if (texture != IntPtr.Zero)
            {
                SDL_DestroyTexture(texture);
            }
        }

        public bool LoadTexture(string t_path)
        {
            this.path = Texture.rootTexturePath + t_path;
            texture = SDL_image.IMG_LoadTexture(Engine.renderer, path);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load texture: " + SDL_GetError());
                Console.WriteLine("Texture path: " + this.path);
                return false;
            }

            // get the size of the texture
            int w, h;
            SDL_QueryTexture(texture, out _, out _, out w, out h);
            rect = new Rect(0, 0, w, h);


            return true;
        }

        public override void Draw(Camera camera)
        {
            if (texture == IntPtr.Zero)
            {
                if (path != null)
                {
                    LoadTexture(path);
                }
                else
                {
                    return;
                }
            }
            var root = gameObject.GetPosition();
            Vec2D size = rect.GetSize();
            var srcRect = rect.ToSDLRect();
            var dstRect = camera.RectToScreen(rect, root).ToSDLRect();

            double time = Time.time;
            double angle = time * 0.3 * 360;

            SDL_RenderCopyEx(Engine.renderer, texture, ref srcRect, ref dstRect, angle, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE);

            SDL_RenderDrawRect(Engine.renderer, ref dstRect);
        }
    }


    public class RotatingSquare : Drawable
    {

        SDL_Rect rect;

        public RotatingSquare()
        {
            // load sdl rect
            /*
            rect = new SDL_Rect();

            rect.w = 333;
            rect.h = 333;
            */

        }

        public double rotationsPerSecond = 0.3;


        public override void Draw(Camera camera)
        {
            // draw a square that rotates around its center
            var renderer = Engine.renderer;
            double time = Time.time;
            double angle = time * 2 * Math.PI * rotationsPerSecond;

            var root = this.gameObject;

            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);

            Vec2D center = new Vec2D(0, 0);
            center = camera.WorldToScreen(center, root.GetPosition());
            // define the square
            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(-250, -250));
            points.Add(new Vec2D(250, -250));
            points.Add(new Vec2D(250, 250));
            points.Add(new Vec2D(-250, 250));

            // rotate the square
            for (int i = 0; i < points.Count; i++)
            {
                // rotate around center
                Vec2D p = points[i];
                p = camera.WorldToScreen(p, root.GetPosition());
                double x = p.x - center.x;
                double y = p.y - center.y;
                points[i] = new Vec2D(
                    x * Math.Cos(angle) - y * Math.Sin(angle),
                    x * Math.Sin(angle) + y * Math.Cos(angle)
                );
            }

            // draw the square
            for (int i = 0; i < points.Count; i++)
            {
                Vec2D p1 = points[i];
                Vec2D p2 = points[(i + 1) % points.Count];
                SDL_RenderDrawLine(renderer, (int)(p1.x + center.x), (int)(p1.y + center.y), (int)(p2.x + center.x), (int)(p2.y + center.y));
            }

            // render the square
            /*
            rect.x = (int)(center.x - rect.w / 2);
            rect.y = (int)(center.y - rect.h / 2);

            SDL_RenderDrawRect(renderer, ref rect);
            */

            
        }
    }
}
