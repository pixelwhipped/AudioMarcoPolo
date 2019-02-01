using Microsoft.Xna.Framework;
using System;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class ColorSparkler : IParticleModifier
    {
        private Tween _t;
        private readonly int _max;
        private readonly int _min;
        public ColorSparkler(int minMil = 100, int maxMil = 1000)
        {

            _min = Math.Min(minMil, maxMil);
            _max = Math.Max(minMil, maxMil);
            _t = new Tween(TimeSpan.FromMilliseconds(BaseGame.Random.Next(minMil, maxMil)), 0f, 1f);
        }

        public void Update(GameTime gameTime, Particle p)
        {
            _t.Update(gameTime.ElapsedGameTime);
            if (_t.IsComplete)
            {
                _t = new Tween(TimeSpan.FromMilliseconds(BaseGame.Random.Next(_min, _max)), 0f, 1f);
            }
            if (_t.Value > 0.5f)
            {
                p.Color *= 0f;
            }
        }
    }
}
