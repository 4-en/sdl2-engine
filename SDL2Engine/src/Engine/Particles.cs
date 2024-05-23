

using Newtonsoft.Json;

namespace SDL2Engine
{
    public class ParticleSystem : Script
    {
        [JsonProperty]
        private Prototype? _particlePrototype;
        [JsonIgnore]
        private List<GameObject> _particles = new List<GameObject>();
        [JsonProperty]
        private int _maxParticles = 100;
        [JsonIgnore]
        private List<GameObject> _activeParticles = new List<GameObject>();
        [JsonProperty]
        private double _particleLifetime = 1.0f;


        public override void Start()
        {
        }

        public override void Update()
        {

        }
    }
}
