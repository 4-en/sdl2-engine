using SDL2Engine;


namespace ShootEmUp
{
    internal class Background : Script
    {
        protected Vec2D gameBounds = new Vec2D(1920, 1080);
        public int BgTileSize = 1024;
        private List<(GameObject background, Vec2D originalPosition)>? prototype = new List<(GameObject, Vec2D)>();
        private GameObject? backgrounds;

        public override void Start()
        {
            var backgroundss = new GameObject("Player");
            backgroundss.AddComponent<Backgrounds>();
            backgrounds = backgroundss;
        }
        bool loaded = false;
        public override void Update()
        {
            if (loaded == false)
            {
                loaded = true;
                var camera = GetCamera() as Camera;

                if (camera == null || prototype == null) return;

                Vec2D cameraPosition2 = camera.GetPosition();

                int roundedCameraX = (int)Math.Ceiling(cameraPosition2.x);
                int roundedCameraY = (int)Math.Ceiling(cameraPosition2.y);
                Vec2D cameraPosition = new Vec2D(roundedCameraX, roundedCameraY);

                if (backgrounds != null)
                {
                    Vec2D backgroundsPosition = backgrounds.GetPosition();
                    // Berechne die neue Position des Hintergrundobjekts relativ zur Kameraposition
                    float xOffset = (int)(backgroundsPosition.x - (gameBounds.x / 2));
                    float yOffset = (int)(backgroundsPosition.y - (gameBounds.y / 2));

                    // Setze die Position relativ zur Kameraposition
                    backgrounds.transform.position = new Vec2D((cameraPosition.x / 1.5) + xOffset, (cameraPosition.y / 1.5) + yOffset);
                }

                loaded = false;
            }
        }
    }
    public class Backgrounds : Script
    {
        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        public int BgTileSize = 1024;
        private List<(GameObject background, Vec2D originalPosition)>? prototype = new List<(GameObject, Vec2D)>();

        public override void Start()
        {
            var backgrounds = new GameObject("Player");
            backgrounds.AddComponent<Backgrounds>();
            int rows = 3; // Anzahl der Reihen
            int columns = 4; // Anzahl der Spalten
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
        }
    }
}

