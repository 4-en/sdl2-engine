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

            lastAction = strength * difficulty + lastAction * (1 - difficulty);

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
}
