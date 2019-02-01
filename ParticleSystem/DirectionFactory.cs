using Microsoft.Xna.Framework;
using System;

namespace AudioMarcoPolo.ParticleSystem
{
    public static class DirectionFactory
    {
        public static Vector2 RandomDirection(float speed)
        {
            var velocity = new Vector2((float)BaseGame.Random.NextDouble() * speed, 0);
            var angle = MathHelper.ToRadians(BaseGame.Random.Next(360));
            return Vector2.Transform(velocity, Matrix.CreateRotationZ(angle));
        }
        public static Vector2 ArcDirection(float speed, float mina, float maxa)
        {
            var a = (MathHelper.Distance(mina, maxa) * (float)BaseGame.Random.NextDouble()) + Math.Min(mina, maxa);
            var velocity = new Vector2((float)BaseGame.Random.NextDouble() * speed, 0);
            var angle = MathHelper.ToRadians(a);
            return Vector2.Transform(velocity, Matrix.CreateRotationZ(angle));
        }
        public static Vector2 RadialDirection(float speed, Vector2 a, Vector2 b)
        {
            var velocity = new Vector2((float)BaseGame.Random.NextDouble() * speed, 0);
            double dy = (b.Y - a.Y);
            double dx = (b.X - a.X);
            var theta = Math.Atan2(dx, dy);
            var angle = MathHelper.ToRadians((float)((90 - ((theta * 180) / Math.PI)) % 360));
            return Vector2.Transform(velocity, Matrix.CreateRotationZ(angle));
        }
    }
}
