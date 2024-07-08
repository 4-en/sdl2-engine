using SDL2Engine;

namespace TileBasedGame.Entities
{

    internal enum BaseAIStates
    {
        IDLE,
        LEFT,
        RIGHT
    }
    public class Enemy : Entity
    {

        public override void Start()
        {
            base.Start();

            var renderer = AddComponent<FilledRect>();
            renderer.SetRect(new Rect(0, 0, 12, 12));
            renderer.color = new Color(255, 0, 0, 255);
            renderer.anchorPoint = AnchorPoint.BottomCenter;
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;


        }
        double lastStateChange = 0;
        BaseAIStates currentState = BaseAIStates.IDLE;
        public override void Update()
        {
            base.Update();
            if (lastStateChange + 2 < Time.time)
            {
                lastStateChange = Time.time;
                currentState = (BaseAIStates)random.Next(0, 3);
            }


            switch (currentState)
            {
                case BaseAIStates.IDLE:
                    Decellerate();
                    break;
                case BaseAIStates.LEFT:
                    MoveLeft();
                    break;
                case BaseAIStates.RIGHT:
                    MoveRight();
                    break;
            }
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            base.OnCollisionEnter(collision);

            IDamageable? damageable = collision.GetOther(gameObject).GetComponent<IDamageable>();
            if(damageable != null && damageable.GetTeam() != team)
            {
                damageable.Damage(new Damage(100, gameObject, team));
            }
        }
    }
}
