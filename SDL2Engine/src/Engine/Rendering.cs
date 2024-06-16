using Newtonsoft.Json;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine
{

    // Used to calculate origin relative to GameObject position
    [Serializable]
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

    [Serializable]
    public struct Color
    {
        [JsonProperty]
        public UInt32 color;

        public byte r
        {
            set
            {
                color = (color & 0x00FFFFFF) | (UInt32)value << 24;
            }

            get
            {
                return (byte)((color & 0xFF000000) >> 24);
            }
        }

        public byte g
        {
            set
            {
                color = (color & 0xFF00FFFF) | (UInt32)value << 16;
            }

            get
            {
                return (byte)((color & 0x00FF0000) >> 16);
            }
        }

        public byte b
        {
            set
            {
                color = (color & 0xFFFF00FF) | (UInt32)value << 8;
            }

            get
            {
                return (byte)((color & 0x0000FF00) >> 8);
            }
        }

        public byte a
        {
            set
            {
                color = (color & 0xFFFFFF00) | (UInt32)value;
            }

            get
            {
                return (byte)(color & 0x000000FF);
            }
        }

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            color = (UInt32)(r << 24 | g << 16 | b << 8 | a);
        }

        public Color(UInt32 color)
        {
            this.color = color;
        }

        public Color() : this(0, 0, 0, 255)
        {
        }

        public SDL.SDL_Color ToSDLColor()
        {
            return new SDL.SDL_Color() { r = r, g = g, b = b, a = a };
        }

        public static Color White = new Color(255, 255, 255, 255);
        public static Color Black = new Color(0, 0, 0, 255);
        public static Color Red = new Color(255, 0, 0, 255);
        public static Color Green = new Color(0, 255, 0, 255);
        public static Color Blue = new Color(0, 0, 255, 255);
        public static Color Magenta = new Color(255, 0, 255, 255);
        public static Color Yellow = new Color(255, 255, 0, 255);
        public static Color Cyan = new Color(0, 255, 255, 255);
        public static Color Gold = new Color(255, 215, 0, 255);
        public static Color Transparent = new Color(0, 0, 0, 0);

        public static bool operator ==(Color a, Color b)
        {
            return a.color == b.color;
        }

        public override bool Equals(object? obj)
        {
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.color != b.color;
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


    public class Camera : Component
    {
        [JsonProperty]
        private Vec2D Position { get; set; }
        [JsonProperty]
        public Vec2D WorldSize { get; set; }
        [JsonProperty]
        public bool keepAspectRatio = true;

        [JsonIgnore]
        private Camera? fixedCamera = null;

        private static Camera backupCamera = new Camera();

        public Camera Fixed
        {
            get
            {
                if (fixedCamera == null)
                {
                    fixedCamera = new Camera();
                    fixedCamera.WorldSize = this.WorldSize;
                    fixedCamera.keepAspectRatio = this.keepAspectRatio;
                }
                return fixedCamera;
            }
        }

        public Camera()
        {
            this.Position = new Vec2D();
            this.WorldSize = new Vec2D(1920, 1080);
        }


        public Camera(Vec2D position = new Vec2D())
        {
            this.Position = position;
            this.WorldSize = new Vec2D(1920, 1080);
        }

        public static Camera GetBackupCamera()
        {
            return backupCamera;
        }


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

        public double GetScreenWidth()
        {
            return GetScreenSize().x;
        }

        public double GetScreenHeight()
        {
            return GetScreenSize().y;
        }

        public double GetWorldWidth()
        {
            return GetWorldSize().x;
        }

        public double GetWorldHeight()
        {
            return GetWorldSize().y;
        }
        bool shouldFollowX = true; // Initialer Zustand, Kamera folgt dem Spieler auf der X-Achse
        bool shouldFollowY = true; // Initialer Zustand, Kamera folgt dem Spieler auf der Y-Achse

        public void SetPosition(Vec2D position)
        {
            this.Position = position;
        }

        public Vec2D GetPosition()
        {
            return this.Position;
        }

        public Vec2D GetWorldSize()
        {
            return WorldSize;
        }

        public Vec2D GetScreenSize()
        {
            if (keepAspectRatio)
            {
                double aspectRatio = WorldSize.x / WorldSize.y;
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

        public Vec2D GetVisibleSize()
        {

            if (keepAspectRatio)
            {
                double aspectRatio = WorldSize.x / WorldSize.y;
                double windowAspectRatio = (double)Engine.windowWidth / (double)Engine.windowHeight;

                if (aspectRatio > windowAspectRatio)
                {
                    return new Vec2D(WorldSize.x, WorldSize.y / (windowAspectRatio / aspectRatio));
                }
                else
                {
                    return new Vec2D(WorldSize.x * (windowAspectRatio / aspectRatio), WorldSize.y);
                }


            }

            return WorldSize;
        }

        public double GetVisibleWidth()
        {
            return GetVisibleSize().x;
        }

        public double GetVisibleHeight()
        {
            return GetVisibleSize().y;
        }

        public Vec2D ScreenToWorld(Vec2D screenPosition)
        {
            Vec2D screenSize = GetScreenSize();
            return new Vec2D(screenPosition.x / screenSize.x * WorldSize.x + Position.x, screenPosition.y / screenSize.y * WorldSize.y + Position.y);
        }

        public Vec2D WorldToScreen(Vec2D worldPosition, Vec2D rootPosition = new Vec2D())
        {
            Vec2D screenSize = GetScreenSize();
            return new Vec2D((worldPosition.x + rootPosition.x - Position.x) / WorldSize.x * screenSize.x, (worldPosition.y + rootPosition.y - Position.y) / WorldSize.y * screenSize.y);
        }

        public Rect RectToScreen(Rect rect, Vec2D rootPosition = new Vec2D())
        {
            Vec2D topLeft = WorldToScreen(rect.GetTopLeft(), rootPosition);
            Vec2D bottomRight = WorldToScreen(rect.GetBottomRight(), rootPosition);
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }

        public Rect RectToWorld(Rect rect, Vec2D rootPosition = new Vec2D())
        {
            Vec2D topLeft = ScreenToWorld(rect.GetTopLeft());
            Vec2D bottomRight = ScreenToWorld(rect.GetBottomRight());
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }

        // converts a distance in world coordinates to screen coordinates
        // we assume that the aspect ratio is kept, otherwise this wouldn't work with
        // just a single distance value
        public double WorldToScreen(double distance)
        {
            return distance / WorldSize.x * GetScreenSize().x;
        }

        public Rect WorldToScreen(Rect worldRect)
        {
            return this.RectToScreen(worldRect);
        }

        public double ScreenToWorld(double distance)
        {
            return distance / GetScreenSize().x * WorldSize.x;
        }

        public Rect ScreenToWorld(Rect screenRect)
        {
            return this.RectToWorld(screenRect);
        }
    }

    // Base class for all drawable components
    // these components are called by their GameObjects to draw themselves
    public class Drawable : Component
    {
        [JsonProperty]
        public AnchorPoint anchorPoint = AnchorPoint.Center;
        [JsonProperty]
        public bool relativeToCamera = true;
        [JsonProperty]
        public int z_index = 0;

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
                return this.relativeToCamera ? camera.WorldToScreen(goRoot) : camera.Fixed.WorldToScreen(goRoot);
            }

            return goRoot;

        }

        public virtual string TextureBatchingCompareKey()
        {
            return "";
        }

        public virtual bool IsVisible(Rect worldRect)
        {
            return worldRect.Contains(gameObject.GetPosition());
        }

        public void SetZIndex(int z_index)
        {
            this.z_index = z_index;
        }
    }

    public class DrawableRect : Drawable
    {
        [JsonProperty]
        protected Rect rect = new Rect(0, 0, 64, 64);
        [JsonProperty]
        public Color color = new Color(255, 255, 255, 255);

        public override Vec2D GetDrawRoot()
        {
            Vec2D localCenter = GetRect().GetTopLeft();
            //return scene?.GetCamera().WorldToScreen(localCenter, gameObject.GetPosition()) ?? localCenter + gameObject.GetPosition();
            Camera cam = this.relativeToCamera ? GetCamera() : GetCamera().Fixed;
            return cam.WorldToScreen(localCenter, gameObject.GetPosition());

        }

        public override bool IsVisible(Rect worldRect)
        {
            return worldRect.Intersects(this.GetWorldRect());
        }

        public void SetRect(Rect rect)
        {
            this.rect = rect;
        }

        // get the rect based on the anchor point in local coordinates
        // for example, if the anchor point is Center, the rect will be centered around the anchor point (move the rect to the left and up by half its size)
        public Rect GetRect(Rect? custom_rect = null)
        {
            // return rect relative to anchor point
            Vec2D size = custom_rect?.GetSize() ?? rect.GetSize();
            Vec2D position = custom_rect?.GetTopLeft() ?? rect.GetTopLeft();

            switch (this.anchorPoint)
            {
                case AnchorPoint.Center:
                    position = new Vec2D(-size.x / 2, -size.y / 2) + position;
                    break;
                case AnchorPoint.TopCenter:
                    position = new Vec2D(-size.x / 2, 0) + position;
                    break;
                case AnchorPoint.TopRight:
                    position = new Vec2D(-size.x, 0) + position;
                    break;
                case AnchorPoint.CenterLeft:
                    position = new Vec2D(0, -size.y / 2) + position;
                    break;
                case AnchorPoint.CenterRight:
                    position = new Vec2D(-size.x, -size.y / 2) + position;
                    break;
                case AnchorPoint.BottomLeft:
                    position = new Vec2D(0, -size.y) + position;
                    break;
                case AnchorPoint.BottomCenter:
                    position = new Vec2D(-size.x / 2, -size.y) + position;
                    break;
                case AnchorPoint.BottomRight:
                    position = new Vec2D(-size.x, -size.y) + position;
                    break;
            }
            return new Rect(position.x, position.y, size.x, size.y);
        }
        public SDL.SDL_Rect GetDestRect(Rect? custom_rect = null)
        {
            return GetScreenRect(custom_rect).ToSDLRect();
        }

        // get the rect in world coordinates
        public Rect GetWorldRect(Rect? custom_rect = null)
        {
            return (custom_rect ?? GetRect()) + gameObject.GetPosition();
        }

        public Rect GetScreenRect(Rect? custom_rect = null)
        {
            var scene = gameObject.GetScene();
            Camera? camera = scene?.GetCamera();
            if (camera != null)
            {
                Rect cam_rect = camera.RectToScreen(GetRect(custom_rect), gameObject.GetPosition());
                if (relativeToCamera)
                {
                    return cam_rect;
                }
                return camera.Fixed.RectToScreen(GetRect(custom_rect), gameObject.GetPosition());

            }
            return GetRect(custom_rect);
        }

        public override void Draw(Camera camera)
        {
            var renderer = Engine.renderer;

            var world_rect = this.GetWorldRect();
            world_rect = camera.RectToScreen(world_rect);

            Vec2D topLeft = new Vec2D(world_rect.x, world_rect.y);
            Vec2D topRight = new Vec2D(world_rect.x + world_rect.w, world_rect.y);
            Vec2D bottomRight = new Vec2D(world_rect.x + world_rect.w, world_rect.y + world_rect.h);
            Vec2D bottomLeft = new Vec2D(world_rect.x, world_rect.y + world_rect.h);


            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            SDL_RenderDrawLine(renderer, (int)topLeft.x, (int)topLeft.y, (int)topRight.x, (int)topRight.y);
            SDL_RenderDrawLine(renderer, (int)topRight.x, (int)topRight.y, (int)bottomRight.x, (int)bottomRight.y);
            SDL_RenderDrawLine(renderer, (int)bottomRight.x, (int)bottomRight.y, (int)bottomLeft.x, (int)bottomLeft.y);
            SDL_RenderDrawLine(renderer, (int)bottomLeft.x, (int)bottomLeft.y, (int)topLeft.x, (int)topLeft.y);
        }

    }

    public class TextRenderer : DrawableRect, ILoadable
    {
        [JsonProperty]
        private string text = "";
        [JsonProperty]
        private int fontSize = 24;
        [JsonProperty]
        private string fontPath = "Assets/Fonts/Roboto-Regular.ttf";
        [JsonProperty]
        private bool updateTexture = true;
        [JsonProperty]
        private bool updateFont = true;
        [JsonIgnore]
        private Font? font;
        [JsonIgnore]
        private IntPtr[]? textures = null;
        [JsonIgnore]
        private Rect[]? textureRects = null;
        [JsonProperty]
        private Rect preferredSize = new Rect(0, 0, 0, 0);
        [JsonIgnore]
        private Rect textTextureSize;
        [JsonProperty]
        private double borderSize = 0;
        [JsonProperty]
        private Color backgroundColor = new Color(0, 0, 0, 0);
        [JsonProperty]
        private Color borderColor = new Color(0, 0, 0, 0);

        public TextRenderer()
        {
            this.relativeToCamera = false;
        }

        public void SetBorderSize(double borderSize)
        {
            if (this.borderSize == borderSize) return;
            this.borderSize = borderSize;
            updateTexture = true;
        }

        public void SetBackgroundColor(Color color)
        {
            if (color == backgroundColor) return;
            this.backgroundColor = color;
            updateTexture = true;
        }

        public void SetColor(Color color)
        {
            if (color == this.color) return;
            this.color = color;
            updateTexture = true;
        }

        public void SetBorderColor(Color color)
        {
            if (color == borderColor) return;
            this.borderColor = color;
            updateTexture = true;
        }

        public void SetPreferredSize(Rect size)
        {
            this.preferredSize = size;
            updateTexture = true;
        }

        public void SetTextSize(Rect size)
        {
            this.textTextureSize = size;
            updateTexture = true;
        }


        public void SetText(string text)
        {
            bool changed = this.text != text;
            this.text = text;
            updateTexture = changed || updateTexture;
        }

        public string GetText()
        {
            return this.text;
        }

        public void SetFontSize(int fontSize)
        {
            this.fontSize = fontSize;
            updateTexture = true;
            updateFont = true;
        }

        public void SetFontPath(string fontPath)
        {
            this.fontPath = fontPath;
            updateTexture = true;
            updateFont = true;
        }

        private void LoadFont()
        {
            if (font != null && !updateFont)
            {
                return;
            }

            if (font != null)
            {
                font.Dispose();
                font = null;
            }

            font = AssetManager.LoadAsset<Font>(fontPath + "@" + fontSize);
            font.Load();
            updateFont = false;
        }

        private void CreateTextTexture()
        {
            if (!IsLoaded() || this.font == null || this.updateFont)
            {
                LoadFont();
            }

            if (textures != null)
            {
                foreach (var texture in textures)
                {
                    if (texture != IntPtr.Zero)
                    {
                        SDL_DestroyTexture(texture);
                    }
                }
            }

            if (font == null)
            {
                return;
            }

            Rect out_rect;
            string[] texts = text.Split('\n');
            textures = new IntPtr[texts.Length];
            textureRects = new Rect[texts.Length];
            this.textTextureSize = new Rect(0, 0, 0, 0);
            for (int i = 0; i < texts.Length; i++)
            {
                textures[i] = font.RenderTexture(texts[i], color.ToSDLColor(), out out_rect);
                this.textTextureSize.w = Math.Max(this.rect.w, out_rect.w);
                this.textTextureSize.h += out_rect.h + 10;
                textureRects[i] = out_rect;
            }

            this.rect.h = Math.Max(this.textTextureSize.h, this.preferredSize.h);
            this.rect.w = Math.Max(this.textTextureSize.w, this.preferredSize.w);

            this.updateTexture = false;
        }

        public override bool IsVisible(Rect worldRect)
        {
            return true;
        }

        public override void Draw(Camera camera)
        {
            // Render Text
            if (updateTexture)
            {
                CreateTextTexture();
            }

            if (textures == null || textureRects == null)
            {
                return;
            }


            var renderer = Engine.renderer;
            var bg_rect = GetDestRect();
            if (backgroundColor.a > 0)
            {
                SDL_SetRenderDrawColor(renderer, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                SDL_RenderFillRect(renderer, ref bg_rect);
            }


            // draw the texture
            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                var out_rect = textureRects[i];
                out_rect.y += i * (out_rect.h + 10);
                var dst_rect = GetDestRect(out_rect);

                SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dst_rect);
            }

            if (borderSize > 0 && borderColor.a > 0)
            {
                SDL_SetRenderDrawColor(renderer, borderColor.r, borderColor.g, borderColor.b, borderColor.a);

                // draw lines around bg_rect as fill rect
                var lineTop = new SDL_Rect() { x = bg_rect.x, y = bg_rect.y, w = bg_rect.w, h = (int)borderSize };
                var lineBottom = new SDL_Rect() { x = bg_rect.x, y = bg_rect.y + bg_rect.h - (int)borderSize, w = bg_rect.w, h = (int)borderSize };
                // dont overlap top or bottom lines
                var lineLeft = new SDL_Rect() { x = bg_rect.x, y = bg_rect.y + (int)borderSize, w = (int)borderSize, h = bg_rect.h - 2 * (int)borderSize };
                var lineRight = new SDL_Rect() { x = bg_rect.x + bg_rect.w - (int)borderSize, y = bg_rect.y + (int)borderSize, w = (int)borderSize, h = bg_rect.h - 2 * (int)borderSize };

                SDL_RenderFillRect(renderer, ref lineTop);
                SDL_RenderFillRect(renderer, ref lineBottom);
                SDL_RenderFillRect(renderer, ref lineLeft);
                SDL_RenderFillRect(renderer, ref lineRight);

            }

        }

        public override void Dispose()
        {
            base.Dispose();
            if (textures != null)
            {
                foreach (var texture in textures)
                {
                    if (texture != IntPtr.Zero)
                    {
                        SDL_DestroyTexture(texture);
                    }
                }
            }


            if (font != null)
            {
                font.Dispose();
                font = null;
            }
        }

        public void Load()
        {
            LoadFont();
        }

        public bool IsLoaded()
        {
            return font != null;
        }
    }

    // Helper class to render text and handle events
    public class TextRenderHelper : Script
    {
        [JsonProperty]
        TextRenderer? textRenderer;
        [JsonIgnore]
        bool wasHovered = false;
        public override void Start()
        {
            textRenderer = GetComponent<TextRenderer>();
        }

        public EventHandler<TextRenderer>? OnClick;
        public EventHandler<TextRenderer>? OnHover;
        public EventHandler<TextRenderer>? OnLeave;

        public override void Update()
        {
            if (textRenderer == null)
            {
                return;
            }


            Rect rect = textRenderer.GetScreenRect();
            bool hovered = SDL2Engine.Utils.MouseHelper.IsRectHovered(rect);
            bool clicked = SDL2Engine.Utils.MouseHelper.IsRectClicked(rect);
            bool left = !hovered && wasHovered;
            wasHovered = hovered;

            if (hovered)
            {
                OnHover?.Invoke(this, textRenderer);
            }

            if (clicked)
            {
                OnClick?.Invoke(this, textRenderer);
            }

            if (left)
            {
                OnLeave?.Invoke(this, textRenderer);
            }


        }


    }

    public class FilledRect : DrawableRect
    {
        public override void Draw(Camera camera)
        {
            var renderer = Engine.renderer;

            var sdl_rect = this.GetDestRect();

            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            SDL_RenderFillRect(renderer, ref sdl_rect);
        }
    }

    public class TextureRenderer : DrawableRect, ILoadable
    {
        [JsonIgnore]
        private Texture? texture;
        [JsonIgnore]
        private Rect source_rect = new Rect(0, 0, 1, 1);
        [JsonProperty]
        public string source = "";

        public override string TextureBatchingCompareKey()
        {
            return source;
        }

        public void SetSource(string source)
        {
            this.source = source;
            if (texture != null)
            {

                texture.Dispose();
                texture = null;
            }
        }

        public bool IsLoaded()
        {
            return texture != null;
        }

        public void Load()
        {
            if (texture != null)
            {
                return;
            }

            if (source != "")
            {
                texture = AssetManager.LoadTexture(source);
                texture.Load();
                this.source_rect = texture.GetTextureRect() ?? new Rect(0, 0, 64, 64);
                this.rect = this.source_rect * 1;
            }
        }

        public void LoadTexture(string path)
        {
            this.SetSource(path);
            this.Load();
        }

        public override bool IsVisible(Rect worldRect)
        {
            if (!this.relativeToCamera)
            {
                return true;
            }
            return base.IsVisible(worldRect);
        }

        public override void Draw(Camera camera)
        {
            // not sure if this should be necessary
            // maybe it should be assumed that the texture is loaded
            if (texture == null)
            {
                this.Load();

                if (texture == null)
                {
                    return;
                }
            }

            var texture_ptr = texture.Get();


            var srcRect = this.source_rect.ToSDLRect();
            var dstRect = this.GetDestRect();

            double angle = gameObject.transform.rotation;

            SDL_RenderCopyEx(Engine.renderer, texture_ptr, ref srcRect, ref dstRect, angle, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE);

        }
    }
    [Serializable]
    public class AnimationInfo
    {
        [JsonProperty]
        public string name;
        [JsonProperty]
        public List<int> frames;
        [JsonProperty]
        public double speed;
        [JsonProperty]
        public AnimationType type;
        [JsonProperty]
        public int direction = 1;

        private AnimationInfo()
        {
            this.name = "";
            this.frames = new List<int>();
            this.speed = 0.1;
            this.type = AnimationType.Loop;
        }

        public AnimationInfo(string name, int frame, double speed = 0.1)
        {
            this.name = name;
            this.frames = new List<int>() { frame };
            this.speed = speed;
            this.type = AnimationType.Loop;
        }

        public AnimationInfo(string name, int firstFrame, int frameCount, double speed = 0.1)
        {
            this.name = name;
            this.frames = new List<int>();
            for (int i = 0; i < frameCount; i++)
            {
                this.frames.Add(firstFrame + i);
            }
            this.speed = speed;
            this.type = AnimationType.Loop;
        }

        public AnimationInfo(string name, List<int> frames, double speed)
        {
            this.name = name;
            this.frames = frames;
            this.speed = speed;
            this.type = AnimationType.Loop;
            this.frames = frames;
        }

        public AnimationInfo(string name, List<int> frames, double speed, AnimationType type)
        {
            this.name = name;
            this.frames = frames;
            this.speed = speed;
            this.type = type;
            this.frames = frames;
        }

        public int firstFrame
        {
            get
            {
                return frames[0];
            }
        }

        public int lastFrame
        {
            get
            {
                return frames[frames.Count - 1];
            }
        }

        public int frameCount
        {
            get
            {
                return frames.Count;
            }
        }

        public void Reverse()
        {
            direction = -direction;
            //frames.Reverse();
        }

    }
    [Serializable]
    public enum AnimationType
    {
        Loop,
        Once,
        OnceAndHold,
        LoopReversed,
        OnceAndDestroy
    }

    /*
     * This class is used to render sprites from a spritesheet,
     * as well as animations that can be defined using the AnimationInfo class
     */
    public class SpriteRenderer : DrawableRect, ILoadable
    {
        [JsonIgnore]
        private Texture? texture;
        [JsonProperty]
        private string source = "";
        [JsonIgnore]
        private Rect source_rect = new Rect(0, 0, 1, 1);
        [JsonProperty]
        private Vec2D spriteSize = new Vec2D(-1, -1);
        [JsonProperty]
        private int spriteIndex = 0;
        [JsonProperty]
        private Dictionary<string, AnimationInfo> animations = new Dictionary<string, AnimationInfo>();
        [JsonProperty]
        private string currentAnimation = "";
        [JsonProperty]
        private string previousAnimation = "";
        [JsonProperty]
        private double animationSpeed = 0.1;
        [JsonProperty]
        private double lastFrameTime = 0;
        [JsonProperty]
        private AnimationType animationType = AnimationType.Loop;
        [JsonProperty]
        private bool flipX = false;
        [JsonProperty]
        private bool flipY = false;
        [JsonProperty]
        private double rotationAngle = 0.0;
        [JsonProperty]
        private bool customWorldSize = false;

        // Sets the size of individual sprites in the spritesheet
        // This is used to calculate the correct source rect for the sprite, based on an index
        public void SetSpriteSize(Vec2D size)
        {
            this.spriteSize = size;
            this.rect = new Rect(0, 0, size.x, size.y);
        }

        public void SetSpriteSize(int width, int height)
        {
            this._tempSetSpriteByCount = new Vec2D(-1, -1);
            this.spriteSize = new Vec2D(width, height);
            this.rect = new Rect(0, 0, width, height);
        }

        public override string TextureBatchingCompareKey()
        {
            return source;
        }

        // temporary variable to set sprite size by count before the texture is loaded
        [JsonIgnore]
        private Vec2D _tempSetSpriteByCount = new Vec2D(-1, -1);
        public void SetSpriteSizeByCount(int rows, int columns)
        {
            if (!this.IsLoaded())
            {
                _tempSetSpriteByCount = new Vec2D(rows, columns);
            }
            else
            {
                this.spriteSize = new Vec2D(this.source_rect.w / columns, this.source_rect.h / rows);
                this.rect = new Rect(0, 0, this.spriteSize.x, this.spriteSize.y);
            }
        }

        // Sets the index of the sprite in the spritesheet
        public void SetSpriteIndex(int index)
        {
            this.spriteIndex = index;
        }

        // Sets the size of the rendered sprite in world coordinates (camera.WorldSize)
        public void SetWorldSize(Vec2D size)
        {
            customWorldSize = true;
            this.rect = new Rect(0, 0, size.x, size.y);
        }

        public void SetWorldSize(int width, int height)
        {
            customWorldSize = true;
            this.rect = new Rect(0, 0, width, height);
        }

        // Sets the size of the rendered sprite in world coordinates (camera.WorldSize)
        public void SetSize(Vec2D size)
        {
            customWorldSize = true;
            this.rect = new Rect(0, 0, size.x, size.y);
        }

        public void SetSize(int width, int height)
        {
            customWorldSize = true;
            this.rect = new Rect(0, 0, width, height);
        }

        // Flips the sprite horizontally
        public void SetFlipX(bool flip)
        {
            this.flipX = flip;
        }

        // Flips the sprite vertically
        public void SetFlipY(bool flip) { this.flipY = flip; }

        // Flips the sprite horizontally and vertically
        public void SetFlip(bool flipX, bool flipY)
        {
            this.flipX = flipX;
            this.flipY = flipY;
        }
        public void SetRotationAngle(double angle)
        {
            this.rotationAngle = angle;
        }

        // Registers an animation with the given name, starting frame, frame count and speed
        public void AddAnimation(string name, int frame, int frameCount, double speed = 0.1)
        {
            animations[name] = new AnimationInfo(name, frame, frameCount, speed);
        }

        // Registers an animation by using an AnimationInfo object
        public void AddAnimation(AnimationInfo animation)
        {
            animations[animation.name] = animation;
        }

        // changes the current animation to the animation with the given name
        public void SetAnimation(string name, AnimationType? type = null)
        {
            if (currentAnimation == name)
            {
                return;
            }

            if (animations.ContainsKey(name))
            {
                previousAnimation = currentAnimation;
                currentAnimation = name;
                lastFrameTime = Time.time;
                spriteIndex = animations[name].firstFrame;
                animationType = animations[name].type;
                animationSpeed = animations[name].speed;
            }

            if (type != null)
            {
                animationType = type.Value;
            }
        }

        // changes the current animation to the animation with the given name
        public void PlayAnimation(string name, AnimationType? type = null)
        {
            SetAnimation(name, type);
        }

        // changes the current animation to the animation with the given name
        public void Play(string name, AnimationType? type = null)
        {
            SetAnimation(name, type);
        }

        // changes the speed of the current animation
        public void SetAnimationSpeed(double speed)
        {
            this.animationSpeed = speed;
        }

        // changes the type of the current animation
        public void SetAnimationType(AnimationType type)
        {
            this.animationType = type;
        }

        // creates a default sprite to show if no custom animation or index was set
        // if no parameters are given, this is equal to the entire texture/spritesheet
        private void AddDefaultSprite()
        {

            var defAnim = new AnimationInfo("default", 0, 1, 1.0);
            animations["default"] = defAnim;


        }

        // loads the texture from the source path
        public void Load()
        {
            if (texture != null)
            {
                //return;
            }

            if (source != "")
            {
                texture = AssetManager.LoadTexture(source);
                texture.Load();
                this.source_rect = texture.GetTextureRect() ?? new Rect(0, 0, 64, 64);
                //this.rect = this.source_rect * 1;
                this.AddDefaultSprite();
                if (_tempSetSpriteByCount.x != -1)
                {
                    this.spriteSize = new Vec2D(this.source_rect.w / _tempSetSpriteByCount.y, this.source_rect.h / _tempSetSpriteByCount.x);
                    this.rect = new Rect(0, 0, this.spriteSize.x, this.spriteSize.y);
                    this._tempSetSpriteByCount = new Vec2D(-1, -1);
                }
                else if (this.spriteSize.x == -1)
                {
                    this.spriteSize = new Vec2D(this.source_rect.w, this.source_rect.h);
                }

                if (this.currentAnimation == "")
                {
                    this.SetAnimation("default");
                }
                if (!customWorldSize)
                    this.rect = new Rect(0, 0, this.spriteSize.x, this.spriteSize.y);
            }
        }

        // loads a texture from the given path
        public void LoadTexture(string path)
        {
            if (this.source != path)
            {
                this.source = path;
                Load();
            }
        }

        // sets the path to the texture without loading it immediately
        public void SetTexture(string path)
        {
            this.source = path;
        }

        public String GetTexture()
        {
            return this.source;
        }

        // sets the path to the texture without loading it immediately
        public void SetSprite(string path)
        {
            this.source = path;
        }

        // sets the path to the texture without loading it immediately
        public void SetSource(string path)
        {
            this.source = path;
        }

        // true if the texture is loaded
        public bool IsLoaded()
        {
            return texture != null && texture.IsLoaded();
        }

        // draws the sprite
        public override void Draw(Camera camera)
        {
            if (texture == null)
            {
                this.Load();
            }

            if (texture == null)
            {
                return;
            }

            var texture_ptr = texture.Get();

            if (currentAnimation == "")
            {
                return;
            }


            int framesPerRow = (int)(source_rect.w / spriteSize.x);
            int x = spriteIndex % framesPerRow;
            int y = spriteIndex / framesPerRow;

            var temp_dest_rect = new Rect(x * spriteSize.x, y * spriteSize.y, spriteSize.x, spriteSize.y);

            var srcRect = temp_dest_rect.ToSDLRect();
            var dstRect = this.GetDestRect();

            double angle = gameObject.transform.rotation + rotationAngle; // Berechnung des Gesamtwinkels

            var flip = SDL_RendererFlip.SDL_FLIP_NONE;
            if (flipX)
            {
                flip |= SDL_RendererFlip.SDL_FLIP_HORIZONTAL;
            }
            if (flipY)
            {
                flip |= SDL_RendererFlip.SDL_FLIP_VERTICAL;
            }

            SDL_RenderCopyEx(Engine.renderer, texture_ptr, ref srcRect, ref dstRect, angle, IntPtr.Zero, flip);


            var animation = animations[currentAnimation];
            double time = Time.time;
            double deltaTime = time - lastFrameTime;
            if (deltaTime > animationSpeed)
            {
                // get next frame index from animation
                spriteIndex += animation.direction;
                lastFrameTime = time;
            }

            if (this.spriteIndex > animation.lastFrame || this.spriteIndex < animation.firstFrame)
            {
                switch (animationType)
                {
                    case AnimationType.Loop:
                        this.spriteIndex = animation.firstFrame;
                        break;
                    case AnimationType.Once:
                        this.SetAnimation(previousAnimation);
                        break;
                    case AnimationType.OnceAndHold:
                        this.spriteIndex = animation.lastFrame;
                        break;
                    case AnimationType.LoopReversed:
                        animation.Reverse();
                        this.spriteIndex += animation.direction;
                        break;
                    case AnimationType.OnceAndDestroy:
                        this.spriteIndex = animation.lastFrame;
                        gameObject.Destroy();
                        break;
                }
            }

        }

        // removes reference to texture and disposes it
        public override void Dispose()
        {
            base.Dispose();
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

    }

    public class TextureRendererOld : DrawableRect
    {
        [JsonIgnore]
        private IntPtr texture_ptr = IntPtr.Zero;
        [JsonIgnore]
        private Texture? texture;
        [JsonProperty]
        private string? path = null;

        ~TextureRendererOld()
        {
            // TODO: check if this actually works
            // seems like the finalizer is not called
            /*
             * This seems to be called before the texture is destroyed and
             * causes access violation errors
             * fix this later with AssetManager
            Console.WriteLine("Texture destroyed");
            if (texture != IntPtr.Zero)
            {
                SDL_DestroyTexture(texture);
            }
            */
        }

        public bool LoadTexture(string t_path)
        {
            // just to test performance without loading the same texture multiple times
            // TODO: implement AssetManager to load and manage assets and remove this
            /*
            if (forsenTexture == IntPtr.Zero)
            {

                this.path = TextureRenderer.rootTexturePath + t_path;
                texture = SDL_image.IMG_LoadTexture(Engine.renderer, path);
                if (texture == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to load texture: " + SDL_GetError());
                    Console.WriteLine("Texture path: " + this.path);
                    return false;
                }
                forsenTexture = texture;
            } else
            {
                texture = forsenTexture;
            }
            */
            texture = AssetManager.LoadAsset<Texture>(t_path);
            texture_ptr = texture.Get();
            // get the size of the texture
            int w, h;
            SDL_QueryTexture(texture_ptr, out _, out _, out w, out h);
            rect = new Rect(0, 0, w, h);


            return true;
        }

        public override void Draw(Camera camera)
        {
            if (texture_ptr == IntPtr.Zero)
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

            var srcRect = rect.ToSDLRect();
            var dstRect = this.GetDestRect();

            double time = Time.time;
            double angle = time * 0.3 * 360;

            SDL_RenderCopyEx(Engine.renderer, texture_ptr, ref srcRect, ref dstRect, angle, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE);

            SDL_RenderDrawRect(Engine.renderer, ref dstRect);
        }
    }


    public class RotatingSquare : Drawable
    {
        [JsonProperty]
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
