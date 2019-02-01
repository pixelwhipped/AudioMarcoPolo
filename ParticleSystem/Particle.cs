using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.ParticleSystem
{
    public delegate Particle ParticleGenerator(Emitter e);
    public interface IParticleModifier
    {
        void Update(GameTime gameTime, Particle p);
    }

    public class Particle
    {
        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector2 Position { get; set; }        // The current position of the particle        
        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance        
        public float Angle { get; set; }            // The current angle of rotation of the particle
        public float AngularVelocity { get; set; }    // The speed that the angle is changing

        public Color Color { get; set; }            // The color of the particle
        public float Size { get; set; }                // The size of the particle
        public float Fade { get; set; }
        public Tween Ttl { get; set; }                // The 'time to live' of the particle
        public List<IParticleModifier> Modifiers;

        public bool Started;
        public bool Ended;
        public Procedure<Particle> OnStart;
        public Procedure<Particle> OnEnd;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, float fade, TimeSpan ttl, params IParticleModifier[] modifiers)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            Ttl = new Tween(ttl, 0f, 1f);
            Fade = fade;
            Modifiers = new List<IParticleModifier>(modifiers);
            OnStart = p => { };
            OnEnd = p => { };
        }
        public void Update(GameTime gameTime)
        {
            Ttl.Update(gameTime.ElapsedGameTime);
            if (!Started)
            {
                Started = true;
                OnStart(this);
            }
            foreach (var m in Modifiers)
            {
                m.Update(gameTime, this);
            }
            Position += Velocity;
            Angle += AngularVelocity;
            if (!Ttl.IsComplete) return;
            Ended = true;
            OnEnd(this);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            var sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            var origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * Fade,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}
