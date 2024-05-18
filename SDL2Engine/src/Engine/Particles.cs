using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    public class ParticleSystem : Script
    {
        private Prototype? _particlePrototype;

        private List<GameObject> _particles = new List<GameObject>();
        private int _maxParticles = 100;
        private List<GameObject> _activeParticles = new List<GameObject>();
        private double _particleLifetime = 1.0f;


        public override void Start()
        {
        }

        public override void Update()
        {

        }
    }
    {
    }
}
