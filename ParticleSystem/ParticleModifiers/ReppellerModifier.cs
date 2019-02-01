using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class ReppellerModifier : IParticleModifier
    {

        private Vector2 Reppeller { get; set; }
        public ReppellerModifier(Vector2 reppeller)
        {
            Reppeller = reppeller;
        }
        public void Update(GameTime gameTime, Particle p)
        {

            var dx = Reppeller - p.Position;
            p.Velocity += (-dx / dx.LengthSquared());

        }
    }
}
