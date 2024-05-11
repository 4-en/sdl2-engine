using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using SDL2Engine;

namespace Pong
{
    public class AIController : Script
    {
        protected PaddleController? paddleController = null;

        protected GameObject? ball = null;
        protected BoxCollider? ballCollider = null;
        protected BoxCollider? paddleCollider = null;
        public double difficulty = 0.5;
        private double lastAction = 0;
        private double nextActionTime = 0;

        public override void Start()
        {
            paddleController = GetComponent<PaddleController>();
            ball = Find("Ball");
            ballCollider = ball?.GetComponent<BoxCollider>();
            paddleCollider = GetComponent<BoxCollider>();
        }

        private bool recalculateAction()
        {
            double time = Time.time;

            if (time > nextActionTime)
            {
                nextActionTime = time + (0.1 - 0.09 * difficulty);
                return true;
            }
            return false;
        }

        protected void setAction(double action)
        {
            if (action == 0)
            {
                lastAction = 0;
                return;
            }

            lastAction = action * difficulty + lastAction * (1 - difficulty);
        }

        protected virtual void calculateAction()
        {
            if (ball == null || paddleController == null || ballCollider == null || paddleCollider == null)
            {
                return;
            }

            double ballY = ball.GetPosition().y + ballCollider.GetCollisionBox().h / 2;
            double paddleY = paddleController.GetGameObject().GetPosition().y + paddleCollider.GetCollisionBox().h / 2;

            double max_error = difficulty * 100;
            double ball_error = random.NextDouble() * max_error - max_error / 2;
            ballY += ball_error;

            double strength = (ballY - paddleY) * 0.01;
            strength = Math.Min(strength, 1);
            strength = Math.Max(strength, -1);

            if (strength > -0.1 && strength < 0.1)
            {
                strength = 0;
            }

            setAction(strength);

        }

        public override void Update()
        {
            if (ball == null || paddleController == null)
            {
                return;
            }

            if (recalculateAction())
            {
                calculateAction();
            }
            paddleController.Move(lastAction);
        }
    }

    public class  BetterAIController : AIController
    {

        public override void Start()
        {
            base.Start();
            difficulty = 0.8;

        }

        private static Vec2D CalculateCollision(Vec2D start, Vec2D direction, Rect rect)
        {
            Vec2D normDir = direction.Normalize();
            double tmin = double.NegativeInfinity;
            double tmax = double.PositiveInfinity;

            if (normDir.x != 0.0d)
            {
                double tx1 = (rect.x - start.x) / normDir.x;
                double tx2 = (rect.x + rect.w - start.x) / normDir.x;

                tmin = Math.Max(tmin, Math.Min(tx1, tx2));
                tmax = Math.Min(tmax, Math.Max(tx1, tx2));
            }

            if (normDir.y != 0.0d)
            {
                double ty1 = (rect.y - start.y) / normDir.y;
                double ty2 = (rect.y + rect.h - start.y) / normDir.y;

                tmin = Math.Max(tmin, Math.Min(ty1, ty2));
                tmax = Math.Min(tmax, Math.Max(ty1, ty2));
            }

            if (tmin > tmax || tmin < 0) return Vec2D.Zero;  // No valid collision

            // Return the point of collision
            return new Vec2D(start.x + tmin * normDir.x, start.y + tmin * normDir.y);
        }

        private double predictImpactY()
        {
            
            if (ball == null || paddleController == null || ballCollider == null || paddleCollider == null)
            {
                return 1080 / 2;
            }

            Vec2D ballPosition = ball.GetPosition();
            Vec2D ballVelocity = ball.GetComponent<PhysicsBody>()?.Velocity ?? Vec2D.Zero;
            if (ballVelocity == Vec2D.Zero)
            {
                return 1080 / 2;
            }
            Rect gameBounds = new Rect(0, 0, 1920, 1080);

            bool paddleIsLeft = paddleController.GetGameObject().GetPosition().x < gameBounds.w / 2;
            Vec2D paddlePosition = paddleController.GetGameObject().GetPosition();
            Rect paddleRect = paddleCollider.GetCollisionBox();
            Vec2D paddleCenter = new Vec2D(paddlePosition.x + paddleRect.w / 2, paddlePosition.y + paddleRect.h / 2);

            Vec2D direction = ballVelocity.Normalize();
            Vec2D collision = CalculateCollision(ballPosition, direction, paddleRect);

            int maxIterations = 5;
            while (collision.x > 10 && collision.x < gameBounds.w - 10)
            {
                if(collision == Vec2D.Zero)
                {
                    return 1080 / 2;
                }

                // Console.WriteLine(collision);

                if(!paddleIsLeft && collision.x > 10)
                {
                    direction.x = -direction.x;
                    collision.x = 11;
                } else if (paddleIsLeft && collision.x < gameBounds.w - 10)
                {
                    direction.x = -direction.x;
                    collision.x = gameBounds.w - 11;
                } else
                {
                    direction.y = -direction.y;
                }

                ballPosition = collision;
                collision = CalculateCollision(ballPosition, direction, paddleRect);

                maxIterations--;
                if (maxIterations <= 0)
                {
                    break;
                }
            }


            if (collision == Vec2D.Zero)
            {
                return 1080 / 2;
            }
            
            return collision.y;


        }

        protected override void calculateAction()
        {
            if (ball == null || paddleController == null || ballCollider == null || paddleCollider == null)
            {
                return;
            }

            double ballY = predictImpactY();
            double paddleHeight = paddleCollider.GetCollisionBox().h;

            double perfectY = ballY - paddleHeight / 2;
            gameObject.SetPosition(new Vec2D(gameObject.GetPosition().x, perfectY));

            double paddleY = paddleController.GetGameObject().GetPosition().y + paddleCollider.GetCollisionBox().h / 2;

            //double max_error = difficulty * 100;
            //double ball_error = random.NextDouble() * max_error - max_error / 2;
            //ballY += ball_error;

            double strength = (ballY - paddleY) * 0.01;
            strength = Math.Min(strength, 1);
            strength = Math.Max(strength, -1);

            if (strength > -0.1 && strength < 0.1)
            {
                strength = 0;
            }

            setAction(0);

        }

    }
}
