using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public static class ParticleFactory
    {
        public static Particle GenerateParticle(Emitter e, Texture2D texture, params Color[] c)
        {

            var ttl = TimeSpan.FromMilliseconds(BaseGame.Random.Next(3000, 7000));
            var cl = new List<Color>(c);
            if (cl.Count == 0)
                cl.Add(new Color((float)BaseGame.Random.NextDouble(), (float)BaseGame.Random.NextDouble(), (float)BaseGame.Random.NextDouble()));

            return new Particle(texture, e.EmissionPoint, DirectionFactory.RandomDirection(1f),
            0.25f, 0.1f * (float)(BaseGame.Random.NextDouble() * 2 - 1), Color.White, 1, 1, ttl, new FountainModifier(0.001f), new ColorFader(ttl, 1f, 0f), new ColorRotator(ttl, cl.ToArray()), new ColorSparkler());
        }
    }
}
