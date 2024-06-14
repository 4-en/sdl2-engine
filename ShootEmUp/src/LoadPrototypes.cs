using SDL2Engine;

namespace ShootEmUp
{
    public static class LoadPrototypes
    {
        private static bool loaded = false;

        public static void Load()
        {
            if (loaded)
            {
                return;
            }
            loaded = true;
            // Load all needed prototypes here by calling functions that create Prototype classes
            LoadExamplePrototype();
            LoadForsenEPrototype();
            BaseEnemy.CreateBasePrototype();
            StaticEnemy.CreateStaticEnemyPrototype();
            Asteroid.CreateAsteroidPrototype();
        }

        private static void LoadExamplePrototype()
        {
            // Create a new prototype
            // prototype name must be unique
            // the prototype is automatically registered with the AssetManager using the prototype name
            var prototype = new Prototype("ExamplePrototype");
            // Add components to the prototype
            prototype.AddComponent<BoxCollider>();

            // afterwards, the prototype can be instantiated using the prototype name or a direct reference to the prototype
            // eg: Prototype.Instantiate("ExamplePrototype"); or prototype.Instantiate();
        }

        private static void LoadForsenEPrototype()
        {
            var forsenEPrototype = new Prototype("forsenE");
            var renderer = forsenEPrototype.GameObject.AddComponent<SpriteRenderer>();
            var circleCollider = forsenEPrototype.GameObject.AddComponent<CircleCollider>();
            circleCollider.SetRadius(100);
            renderer.SetSource("forsenE.png");
        }

    }
}
