using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class BlackHoleModifier : IParticleModifier
    {
        /// <summary>
        /// Gets or sets the center of the gravity point
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the radius of influence
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the strength of the pull of gravity
        /// </summary>
        public float Strength { get; set; }

        public float Horizon { get; set; }
        public BlackHoleModifier(Vector2 pos, float r, float e, float s)
        {
            Position = pos;
            Radius = r;
            Strength = s;
            Horizon = e;
        }

        public void Update(GameTime gameTime, Particle p)
        {
            var distance = Vector2.Subtract(Position, p.Position);
            if (distance.LengthSquared() < Horizon * Horizon)
            {
                p.Ttl.Finish();
            }
            else if (distance.LengthSquared() < Radius * Radius)
            {
                var force = Vector2.Normalize(distance);
                force = Vector2.Multiply(force, Strength);
                force = Vector2.Multiply(force, p.Ttl.CurrentTime.Seconds);

                p.Velocity += force;

            }
        }

    }
}
