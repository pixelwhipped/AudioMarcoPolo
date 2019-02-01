using Microsoft.Xna.Framework;
using System;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class ScaleModifier : IParticleModifier
    {
        private readonly Tween _tween;
        public ScaleModifier(TimeSpan time, float start, float finish)
        {
            _tween = new Tween(time, start, finish);
        }
        public void Update(GameTime gameTime, Particle p)
        {

            _tween.Update(gameTime.ElapsedGameTime);
            p.Size = _tween;

        }
    }
}
