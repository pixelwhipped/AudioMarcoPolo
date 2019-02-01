using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class ForceModifier : IParticleModifier
    {

        private Vector2 Force { get; set; }
        public ForceModifier(Vector2 force)
        {
            Force = force;
        }
        public void Update(GameTime gameTime, Particle p)
        {
            p.Velocity += p.Velocity * Force;
        }
    }
}
