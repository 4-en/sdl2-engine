using SDL2Engine;

namespace TileBasedGame.Items
{
    internal class Coin : Script
    {
        public int value = 1;
        SoundPlayer? coinSound = null;
        public override void Start()
        {
            coinSound = AddComponent<SoundPlayer>();
            coinSound?.Load("Assets/Audio/coin.mp3");
            coinSound?.SetVolume(0.2);

            // add a collider
            var c = BoxCollider.FromDrawableRect(gameObject);
            if (c != null)
            {
                c.IsTrigger = true;
            }
        }

        public override void Update()
        {
        }

        private bool gavePoints = false;
        public override void OnCollisionEnter(CollisionPair collision)
        {

            if (gavePoints) {
                return;
            }
            var other = collision.GetOther(gameObject);

            if(other.GetComponent<Player>() != null)
            {
                gavePoints = true;
                // add score
                EventBus.Dispatch(new PlayerScoreEvent(value));
                // destroy coin
                gameObject.Destroy();

                
                //play sound
                coinSound?.Play();
            }
        }
    }
    
}
