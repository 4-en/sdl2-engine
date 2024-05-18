
namespace SDL2Engine.Rand
{

    public class RandomGenerator
    {

        private uint _seed;
        private uint _state;

        public RandomGenerator()
        {
            _seed = (uint)System.Environment.TickCount;
            _state = _seed;
        }

        public RandomGenerator(uint seed)
        {
            _seed = seed;
            _state = seed;
        }

        public uint Next()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        public uint Next(uint min, uint max)
        {
            return Next() % (max - min) + min;
        }

        public float NextFloat()
        {
            return (float)Next() / uint.MaxValue;
        }

        public double NextDouble()
        {
            return (double)Next() / uint.MaxValue;
        }

    }

    public class StableRandom
    {

        private int seed;
        private int state = 0x6969;

        public StableRandom()
        {
            seed = System.Environment.TickCount;
        }

        public StableRandom(int seed)
        {
            this.seed = seed;
        }

        public int Next()
        {
            int combined = Combine(seed, state, -state);
            state = Hash(combined);
            return state;
        }

        public int Next(int min, int max)
        {
            return Next() % (max - min) + min;
        }

        public float NextFloat()
        {
            return (float)Next() / int.MaxValue;
        }

        public double NextDouble()
        {
            return (double)Next() / int.MaxValue;
        }

        public int Generate(int x, int y)
        {
            // Combine seed and coordinates into a single value
            int combined = Combine(seed, x, y);

            // Use a simple hash function to generate a pseudo-random number
            return Hash(combined);
        }

        private int Combine(int seed, int x, int y)
        {
            int hash = 17;
            hash = hash * 31 + seed;
            hash = hash * 31 + x;
            hash = hash * 31 + y;
            return hash;
        }

        private int Hash(int value)
        {
            value = ((value >> 16) ^ value) * 0x45d9f3b;
            value = ((value >> 16) ^ value) * 0x45d9f3b;
            value = (value >> 16) ^ value;
            return value;
        }
    }

    public class PerlinNoise
    {
        private StableRandom random;

        public PerlinNoise(int seed)
        {
            random = new StableRandom(seed);
        }

        public float Noise(float x, float y, float scale)
        {
            return Noise(x * scale, y * scale);
        }

        public float Noise(float x, float y)
        {
            // Determine grid cell coordinates
            int x0 = (int)Math.Floor(x);
            int x1 = x0 + 1;
            int y0 = (int)Math.Floor(y);
            int y1 = y0 + 1;

            // Interpolate between grid points
            float sx = x - x0;
            float sy = y - y0;

            float n0, n1, ix0, ix1, value;

            n0 = DotGridGradient(x0, y0, x, y);
            n1 = DotGridGradient(x1, y0, x, y);
            ix0 = Interpolate(n0, n1, sx);

            n0 = DotGridGradient(x0, y1, x, y);
            n1 = DotGridGradient(x1, y1, x, y);
            ix1 = Interpolate(n0, n1, sx);

            value = Interpolate(ix0, ix1, sy);
            return value;
        }

        private float DotGridGradient(int ix, int iy, float x, float y)
        {
            // Random gradient vector
            int randomGradient = random.Generate(ix, iy);
            float angle = (randomGradient % 360) * (float)Math.PI / 180f; // Convert to radians
            float gradientX = (float)Math.Cos(angle);
            float gradientY = (float)Math.Sin(angle);

            // Distance vector
            float dx = x - ix;
            float dy = y - iy;

            // Compute the dot-product
            return (dx * gradientX + dy * gradientY);
        }

        private float Interpolate(float a, float b, float t)
        {
            // Smoothstep interpolation
            t = t * t * (3f - 2f * t);
            return a + t * (b - a);
        }
    }
}
