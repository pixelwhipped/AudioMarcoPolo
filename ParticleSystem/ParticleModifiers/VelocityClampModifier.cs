using Microsoft.Xna.Framework;
using System;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class VelocityClampModifier : IParticleModifier
    {
        private Vector2 _max;
        public VelocityClampModifier(Vector2 clamp)
        {
            _max = new Vector2(Math.Abs(clamp.X), Math.Abs(clamp.Y));
        }
        public void Update(GameTime gameTime, Particle p)
        {
            p.Velocity = new Vector2(MathHelper.Clamp(p.Velocity.X, -_max.X, _max.X),
                MathHelper.Clamp(p.Velocity.Y, -_max.Y, _max.Y));
        }
    }
}
