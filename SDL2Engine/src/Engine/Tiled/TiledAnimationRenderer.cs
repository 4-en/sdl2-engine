using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace SDL2Engine
{
    public class TiledAnimationRenderer : DrawableRect, ILoadable
    {
        [JsonIgnore]
        private Texture? texture;
        [JsonProperty]
        private string source = "";
        [JsonProperty]
        private Rect[] source_rects = new Rect[0];
        [JsonProperty]
        private int[] durations = new int[0];
        [JsonProperty]
        private bool flipX = false;
        [JsonProperty]
        private bool flipY = false;
        [JsonProperty]
        private double rotationAngle = 0.0;
        [JsonProperty]
        private bool customWorldSize = false;
        [JsonProperty]
        private int totalDuration = 0;

        
        public override string TextureBatchingCompareKey()
        {
            return source;
        }

        public void ClearAnimations()
        {
            source_rects = new Rect[0];
            durations = new int[0];
            totalDuration = 0;
        }

        public void AddAnimationFrame(Rect source_rect, int duration_ms)
        {
            var new_source_rects = new Rect[source_rects.Length + 1];
            var new_durations = new int[durations.Length + 1];
            for (int i = 0; i < source_rects.Length; i++)
            {
                new_source_rects[i] = source_rects[i];
                new_durations[i] = durations[i];
            }
            new_source_rects[source_rects.Length] = source_rect;
            source_rects = new_source_rects;

            new_durations[durations.Length] = duration_ms;
            durations = new_durations;

            totalDuration += duration_ms;
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
                if(this.source_rects.Length == 0)
                {
                    this.source_rects = new Rect[1];
                    this.durations = new int[1];
                    this.durations[0] = 1000;
                    this.source_rects[0] = texture.GetTextureRect() ?? new Rect(0, 0, 64, 64);
                }

                // if custom world size is not set, set it to the size of the texture
                if (!customWorldSize)
                {
                    this.rect = this.source_rects[0] * 1.0;
                }
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

        public string GetTexture()
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

        private int last_source_index = 0;
        private Rect GetSourceRect()
        {
            int time = (int)(Time.time * 1000 % totalDuration);
            
            int time_count = 0;
            for (int i = 0; i < durations.Length; i++)
            {
                time_count += durations[i];
                if (time < time_count)
                {
                    last_source_index = i;
                    return source_rects[i];
                }
            }

            return source_rects[last_source_index];
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



            var srcRect = GetSourceRect().ToSDLRect();

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
}
