using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.ParticleSystem
{
    public static class ParticleBatch
    {
        public static void Draw(this SpriteBatch spriteBatch, ParticleEngine p)
        {
            p.Draw(spriteBatch);
        }

        public static void Draw(this SpriteBatch spriteBatch, Particle p)
        {
            p.Draw(spriteBatch);
        }

        public static void Draw(this SpriteBatch spriteBatch, Emitter p)
        {
            p.Draw(spriteBatch);
        }
    }
}
