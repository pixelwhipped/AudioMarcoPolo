using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.ParticleSystem
{
    public interface IEmitterModifier
    {
        bool IsPattern { get; }
        void Update(GameTime gameTime, Emitter e);
    }

    public class Emitter
    {
        public Vector2 EmitterLocation { get; set; }
        public Vector2 EmissionPoint { get; set; }
        public List<Particle> Particles;
        public Tween Ttl { get; set; }                // The 'time to live' of the particle
        public ParticleGenerator Generator;

        public bool Started;
        public bool Ended;
        public Procedure<Emitter> OnStart;
        public Procedure<Emitter> OnEnd;
        public int EmmisionRate;
        public Tween EmmitVal;
        public int ParticlelReleaseCount = 0;
        public List<IEmitterModifier> Modifiers;
        public int Max;

        public bool IsComplete
        {
            get
            {
                return Ttl.IsComplete && Particles.Count == 0;
            }
        }
        public Emitter(Vector2 location, TimeSpan ttl, ParticleGenerator generator, int rate, int max, params IEmitterModifier[] modifiers)
        {
            EmitterLocation = location;
            EmissionPoint = location;
            Particles = new List<Particle>();
            Ttl = new Tween(ttl, 0f, 1f);
            EmmisionRate = rate;
            EmmitVal = new Tween(TimeSpan.FromSeconds(1), 0, 1);
            Generator = generator;
            OnStart = p => { };
            OnEnd = p => { };
            Modifiers = new List<IEmitterModifier>(modifiers);
            Max = max;
        }
        public void Update(GameTime gameTime)
        {
            EmissionPoint = EmitterLocation;
            Ttl.Update(gameTime.ElapsedGameTime);
            if (!Started)
            {
                Started = true;
                OnStart(this);
            }
            if (!Ttl.IsComplete && Particles.Count < Max)
            {
                Emit(gameTime);
            }

            foreach (var m in Modifiers.Where(p => !p.IsPattern))
            {
                m.Update(gameTime, this);
            }

            for (var particle = 0; particle < Particles.Count; particle++)
            {
                Particles[particle].Update(gameTime);
                if (!Particles[particle].Ttl.IsComplete) continue;
                Particles.RemoveAt(particle);
                particle--;
            }
            if (Ended || !Ttl.IsComplete) return;
            Ended = true;
            OnEnd(this);
        }
        public void Emit(GameTime gameTime)
        {
            EmmitVal.Update(gameTime.ElapsedGameTime);

            var total = (int)(EmmisionRate * EmmitVal.Value) - ParticlelReleaseCount;

            for (var i = 0; i < total; i++)
            {
                foreach (var m in Modifiers.Where(p => p.IsPattern))
                {
                    m.Update(gameTime, this);
                }
                Particles.Add(Generator(this));
                ParticlelReleaseCount++;
            }
            if (!EmmitVal.IsComplete) return;
            EmmitVal.Reset();
            ParticlelReleaseCount = 0;
        }
        public void Reset()
        {
            Ended = false;
            Started = false;
            Ttl.Reset();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var t in Particles)
            {
                spriteBatch.Draw(t);
            }
        }
    }
}
