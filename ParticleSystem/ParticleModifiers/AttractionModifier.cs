using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class AttractionModifier : IParticleModifier
    {

        private Vector2 Attractor { get; set; }
        public AttractionModifier(Vector2 attractor)
        {
            Attractor = attractor;
        }
        public void Update(GameTime gameTime, Particle p)
        {
            var dx = p.Position - Attractor;
            p.Velocity += (-dx / dx.LengthSquared());
        }
    }
}
