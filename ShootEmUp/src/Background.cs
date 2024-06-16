using SDL2Engine;
using ShootEmUp.Entities;
using ShootEmUp.Level;
using System.Reflection.Metadata;


namespace ShootEmUp
{
    internal class Background : Script
    {
        protected Vec2D gameBounds = new Vec2D(1920, 1080);
        public int BgTileSize = 1500;
        private List<(GameObject background, Vec2D originalPosition)>? prototype = new List<(GameObject, Vec2D)>();
        private GameObject Fire = new GameObject();
        public override void Start()
        {
            int rows = 6; // Anzahl der Reihen
            int columns = 8; // Anzahl der Spalten
            float spacing = BgTileSize - 1; // Abstand zwischen den Objekten bzw. PNG-Größe




            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var background = new GameObject("Background" + (row * columns + col));
                    var bg_renderer = background.AddComponent<SpriteRenderer>();
                    bg_renderer?.SetSource("Assets/Textures/background/Blue01.png");
                    bg_renderer?.SetZIndex(999);
                    bg_renderer?.SetSize(BgTileSize, BgTileSize);

                    // Berechne die Position relativ zur Mitte
                    float xOffset = (col - (columns / 2)) * spacing;
                    float yOffset = (row - (rows / 2)) * spacing;

                    // Korrigiere die x- und y-Offsets, wenn die Anzahl der Spalten oder Reihen gerade ist
                    if (columns % 2 == 0) xOffset += spacing / 2;
                    if (rows % 2 == 0) yOffset += spacing / 2;

                    // Setze die Position relativ zur Mitte des Spielfelds
                    Vec2D position = new Vec2D((gameBounds.x / 2) + xOffset, (gameBounds.y / 2) + yOffset);
                    background.transform.position = position;
                    prototype?.Add((background, position));
                }
            }
            //var Fireboarder = new GameObject("Fireboarder");
            Fire?.AddComponent<Fireboarder>();
        }
        bool loaded = false;

        public override void Update()
        {
            var camera = GetCamera() as Camera;

            if (loaded == false)
            {
                loaded = true;

                if (camera == null || prototype == null) return;

                Vec2D cameraPosition2 = camera.GetPosition();

                int roundedCameraX = (int)Math.Ceiling(cameraPosition2.x);
                int roundedCameraY = (int)Math.Ceiling(cameraPosition2.y);
                Vec2D cameraPosition = new Vec2D(roundedCameraX, roundedCameraY);

                foreach (var (background, originalPosition) in prototype)
                {
                    if (background != null)
                    {
                        // Berechne die neue Position des Hintergrundobjekts relativ zur Kameraposition
                        float xOffset = (int)(originalPosition.x - (gameBounds.x / 2));
                        float yOffset = (int)(originalPosition.y - (gameBounds.y / 2));

                        // Setze die Position relativ zur Kameraposition
                        background.transform.position = new Vec2D((cameraPosition.x / 1.5) + xOffset, (cameraPosition.y / 1.5) + yOffset);
                    }
                }
                loaded = false;
                // Fire.SetPosition(new Vec2D(cameraPosition.x + (gameBounds.x / 2), cameraPosition.y + (gameBounds.y / 2)));
            }

        }
        public class Fireboarder : Script
        {
            public Vec2D worldSize = BaseLevel.WorldSize;

            public override void Start()
            {
                // collision Boarder Top object
                var obstacle1 = new GameObject("Obstacle");
                var bc1 = obstacle1.AddComponent<BoxCollider>();
                bc1.UpdateColliderSize(5000, 300); // Größe des Objekts
                obstacle1.transform.position = new Vec2D(-worldSize.x, worldSize.y - 32);
                int space1 = 0;
                CreateFireBorder(obstacle1.transform.position, new Vec2D(worldSize.x * 2, 300), 32, space1, 0, true); // Top Boarder

                // collision Boarder bottom object
                var obstacle2 = new GameObject("Obstacle");
                var bc2 = obstacle2.AddComponent<BoxCollider>();
                bc2.UpdateColliderSize(5000, 300); // Größe des Objekts
                obstacle2.transform.position = new Vec2D(-2500, -2768);
                int space2 = 300;
                CreateFireBorder(obstacle2.transform.position, new Vec2D(worldSize.x * 2, 300), 32, space2, 180, true); // Bottom Boarder

                // collision Boarder right object
                var obstacle3 = new GameObject("Obstacle");
                var bc3 = obstacle3.AddComponent<BoxCollider>();
                bc3.UpdateColliderSize(300, 5000); // Größe des Objekts
                obstacle3.transform.position = new Vec2D(2468, -2500);
                int space3 = 0;
                CreateFireBorder(obstacle3.transform.position, new Vec2D(300, worldSize.y * 2), 32, space3, 270, false); // Right Boarder

                // collision Boarder left object
                var obstacle4 = new GameObject("Obstacle");
                var bc4 = obstacle4.AddComponent<BoxCollider>();
                bc4.UpdateColliderSize(300, 5000); // Größe des Objekts
                obstacle4.transform.position = new Vec2D(-2768, -2500);
                int space4 = 300;
                CreateFireBorder(obstacle4.transform.position, new Vec2D(300, worldSize.y * 2), 32, space4, 90, false); // Left Boarder
            }

            private void CreateFireBorder(Vec2D obstaclePosition, Vec2D obstacleSize, int spriteSize, int space, int rotation, bool isHorizontal)
            {
                int obstacleWidth = (int)obstacleSize.x;
                int obstacleHeight = (int)obstacleSize.y;
                List<SpriteRenderer> fireRenderers = new List<SpriteRenderer>();

                // Erstelle Sprites entlang der Kante des Objekts
                for (float offset = 0; offset <= (isHorizontal ? obstacleWidth : obstacleHeight); offset += 99)
                {
                    var fireSprite = new GameObject("FireSprite");
                    var fireRenderer = fireSprite.AddComponent<SpriteRenderer>();
                    fireRenderer?.SetTexture("Assets/Textures/FireBoarder32.png");
                    fireRenderer?.SetSpriteSize(spriteSize, spriteSize);
                    fireRenderer?.SetSize(100, 100);
                    fireRenderer?.AddAnimation(new AnimationInfo("fire", 0, 4, 0.075));
                    fireRenderer?.SetAnimationType(AnimationType.LoopReversed);
                    fireRenderer?.SetRotationAngle(rotation);

                    if (fireRenderer != null)
                    {
                        fireRenderers.Add(fireRenderer);
                    }

                    // Berechne die Position des Sprites entlang der Kante
                    float posX = (int)obstaclePosition.x;
                    float posY = (int)obstaclePosition.y;
                    if (isHorizontal)
                    {
                        posX += offset;
                        posY += space;
                    }
                    else
                    {
                        posX += space;
                        posY += offset;
                    }

                    fireSprite.transform.position = new Vec2D(posX, posY);
                }

                foreach (var renderer in fireRenderers)
                {
                    renderer.PlayAnimation("fire");
                }
            }
        }
    }
}

