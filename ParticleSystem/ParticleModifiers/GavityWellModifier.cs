using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class GravityWellModifier : IParticleModifier
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

        public GravityWellModifier(Vector2 pos, float r, float s)
        {
            Position = pos;
            Radius = r;
            Strength = s;
        }

        public void Update(GameTime gameTime, Particle p)
        {
            var distance = Vector2.Subtract(Position, p.Position);

            if (!(distance.LengthSquared() < Radius * Radius)) return;
            var force = Vector2.Normalize(distance);
            force = Vector2.Multiply(force, Strength);
            force = Vector2.Multiply(force, p.Ttl.CurrentTime.Seconds);

            p.Velocity += force;
        }

    }
}
