using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.EmmiterModifiers
{
    public class RandomEmmisionRate : IEmitterModifier
    {
        private bool _started;
        private int _rate;
        public bool IsPattern { get { return false; } }
        public float Randomness;
        public RandomEmmisionRate(float r)
        {
            Randomness = r;
        }
        public void Update(GameTime gameTime, Emitter e)
        {
            if (!_started)
            {
                _rate = e.EmmisionRate;
                _started = true;
            }
            e.EmmisionRate = (int)(_rate * ((float)BaseGame.Random.NextDouble() * Randomness));
        }
    }
}
