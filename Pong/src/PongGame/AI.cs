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

        public enum Strategy
        {
            Center,
            Counter,
            MaxAngle,
            Random
        }

        private Strategy strategy = Strategy.Center;
        private double strategyChangeTime = 0;
        public bool unfair = false;
        // returns y of top, center and bottom of paddle
        public Tuple<double, double, double> GetPaddleY()
        {
            if (paddleCollider == null || ball == null || ballCollider == null || paddleController == null)
            {
                return new Tuple<double, double, double>(1080 / 2-80, 1080 / 2, 1080/2+80);
            }

            Rect paddleRect = paddleCollider.GetCollisionBox();
            Vec2D paddlePosition = paddleController.GetGameObject().GetPosition();
            Vec2D paddleCenter = new Vec2D(paddlePosition.x + paddleRect.w / 2, paddlePosition.y + paddleRect.h / 2);

            return new Tuple<double, double, double>(paddlePosition.y, paddleCenter.y, paddlePosition.y + paddleRect.h);
            
        }

        public override void Start()
        {
            base.Start();
            difficulty = 0.4;

        }

        public override void Update()
        {
            base.Update();

            if(Time.time > strategyChangeTime)
            {
                strategyChangeTime = Time.time + 10 - 7 * difficulty;
                strategy = (Strategy)random.Next(0, 4);
                randStratDouble = random.NextDouble();
            }
        }

        private static Vec2D CalculateCollision(Vec2D start, Vec2D direction, Rect rect)
        {

            // check if start is in rect
            if (start.x >= rect.x + rect.w || start.x <= rect.x || start.y >= rect.y + rect.h || start.y <= rect.y)
            {
                return start;
            }

            Vec2D normDir = direction.Normalize();

            double y_impact_time = (rect.y + rect.h - start.y) / normDir.y;
            if (y_impact_time < 0)
            {
                y_impact_time = (rect.y - start.y) / normDir.y;
            }

            double x_impact_time = (rect.x + rect.w - start.x) / normDir.x;
            if (x_impact_time < 0)
            {
                x_impact_time = (rect.x - start.x) / normDir.x;
            }

            if(Math.Abs(x_impact_time) < Math.Abs(y_impact_time))
            {
                return start + normDir * x_impact_time;
            }
            else
            {
                return start + normDir * y_impact_time;
            }
        }

        private double predictImpactY()
        {
            
            if (ball == null || paddleController == null || ballCollider == null || paddleCollider == null)
            {
                return 1080 / 2;
            }

            Vec2D ballPosition = ball.GetPosition();
            Vec2D ballVelocity = ball.GetComponent<PhysicsBody>()?.Velocity ?? Vec2D.Zero;
            if (ballVelocity.LengthSquared() < 1)
            {
                return 1080 / 2;
            }
            Rect gameBounds = new Rect(100, 0, 1720, 1080);

            bool paddleIsLeft = paddleController.GetGameObject().GetPosition().x < gameBounds.w / 2;
            Vec2D paddlePosition = paddleController.GetGameObject().GetPosition();
            Rect paddleRect = paddleCollider.GetCollisionBox();

            Vec2D direction = ballVelocity.Normalize();
            Vec2D collision = CalculateCollision(ballPosition, direction, gameBounds);
            int max_iterations = (int)(10 * difficulty);
            double x_tolerance = 110;
            while (collision.x > x_tolerance && collision.x < (gameBounds.w - x_tolerance) && max_iterations > 0 ||
                max_iterations > 0 && (( paddleIsLeft && collision.x > gameBounds.w / 2) || (!paddleIsLeft && collision.x < gameBounds.w / 2)))
            {
                Vec2D newDirection = direction;

                if (collision.y < 10 || collision.y > gameBounds.h - 10)
                {
                    newDirection.y = -newDirection.y;
                }
                else
                {
                    newDirection.x = -newDirection.x;
                }

                collision += (newDirection / 60);

                collision = CalculateCollision(collision, newDirection, gameBounds);
                max_iterations--;
            }

            if (collision == Vec2D.Zero)
            {
                return 1080 / 2;
            }
            
            return collision.y;


        }

        private double randStratDouble = 0.5;
        private double ApplyStrategy(double predBallY)
        {
            if(paddleCollider == null)
            {
                return predBallY;
            }

            Tuple<double, double, double> paddleY = GetPaddleY();
            double paddleCenter = paddleY.Item2;
            double paddleTop = paddleY.Item1;
            double paddleBottom = paddleY.Item3;

            double safetyMargin = 10;
            paddleTop += safetyMargin;
            paddleBottom -= safetyMargin;

            double paddleTopOffset = paddleTop - paddleCenter;
            double paddleBottomOffset = paddleBottom - paddleCenter;

            double ballYDirection = ball?.GetComponent<PhysicsBody>()?.Velocity.y ?? 0;


            switch (strategy)
            {
                case Strategy.Center:
                    return predBallY;
                case Strategy.MaxAngle:
                    return ballYDirection > 0 ? predBallY + paddleTopOffset : predBallY + paddleBottomOffset;
                case Strategy.Counter:
                    return ballYDirection < 0 ? predBallY + paddleTopOffset : predBallY + paddleBottomOffset;
                case Strategy.Random:
                    return predBallY + paddleBottomOffset + randStratDouble * (paddleTopOffset - paddleBottomOffset);
                default:
                    return paddleCenter;
            }
        }

        protected override void calculateAction()
        {
            if (ball == null || paddleController == null || ballCollider == null || paddleCollider == null)
            {
                return;
            }

            double predBallY = predictImpactY();
            Vec2D ballCenter = ballCollider.GetCollisionBox().Center();
            double paddleHeight = paddleCollider.GetCollisionBox().h;

            double perfectY = predBallY - paddleHeight / 2;

            if (unfair)
            {
                gameObject.SetPosition(new Vec2D(gameObject.GetPosition().x, perfectY));
                return;
            }

            double ballPaddleDistX = Math.Abs(ballCenter.x - paddleController.GetGameObject().GetPosition().x);
            double directInfluence = Math.Min( 250 , ballPaddleDistX) / 250;
            perfectY = perfectY * directInfluence + ballCenter.y * (1 - directInfluence);

            /*
            double max_error = difficulty * 100;
            double ball_error = random.NextDouble() * max_error - max_error / 2;
            perfectY += ball_error;
            */

            perfectY = ApplyStrategy(perfectY);

            double strength = (perfectY - paddleController.GetGameObject().GetPosition().y) * 0.01;
            strength = Math.Min(strength, 1);
            strength = Math.Max(strength, -1);

            if (strength > -0.1 && strength < 0.1)
            {
                strength = 0;
            }

            setAction(strength);

        }

    }
}
