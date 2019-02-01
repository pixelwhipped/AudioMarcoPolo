using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.Content
{
    public static class Textures
    {
        public static Texture2D ParticleFlowerBurst;
        public static Texture2D ParticleDiamond;
        public static Texture2D ParticleStar;
        public static Texture2D ParticleSmoke;

        public static Texture2D Back;
        public static Texture2D Forward;
        public static Texture2D Cursor;

        public static Texture2D Splat1;
        public static Texture2D Splat2;
        public static Texture2D Splat3;
        public static void LoadContent(ContentManager content)
        {
            ParticleFlowerBurst = content.Load<Texture2D>(@"Particle\FlowerBurst.png");
            ParticleDiamond = content.Load<Texture2D>(@"Particle\Diamond.png");
            ParticleStar = content.Load<Texture2D>(@"Particle\Star.png");            
            ParticleSmoke = content.Load<Texture2D>(@"Particle\Smoke.png");
            Back = content.Load<Texture2D>(@"Images\Back.png");
            Forward = content.Load<Texture2D>(@"Images\Forward.png");
            Cursor = content.Load<Texture2D>(@"Images\Cursor.png");
            Splat1 = content.Load<Texture2D>(@"Splat\1.png");
            Splat2 = content.Load<Texture2D>(@"Splat\2.png");
            Splat3 = content.Load<Texture2D>(@"Splat\3.png");
        }
    }
}
