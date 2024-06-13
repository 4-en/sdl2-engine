using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.src
{
    internal class Shop
    {
        internal static Scene CreateScene()
        {
            var scene = new Scene("Shop Screen");
            
            using (var activeScene = scene.Activate())
            {
                Vec2D gameBounds = new Vec2D(1920, 1080);
                GameObject background = new GameObject("Background");
                background.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/space_background.jpg");
                background.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
                scene.AddGameObject(background);
                var shopmenu = UI.ShopMenu("Shop", () =>
                {
                    return true;
                });
                scene.AddGameObject(shopmenu);
            }
           
            
            return scene;
        }

        
    }
}
