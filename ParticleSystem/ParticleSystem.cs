using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AudioMarcoPolo.ParticleSystem
{
    public class ParticleEngine
    {
        private readonly List<Emitter> _particleSystems;
        public ParticleEngine()
        {
            _particleSystems = new List<Emitter>();
        }
        public void Add(Emitter e)
        {
            _particleSystems.Add(e);
        }
        public void Update(GameTime gameTime)
        {
            foreach (var e in _particleSystems)
            {
                e.Update(gameTime);
            }
            for (var particle = 0; particle < _particleSystems.Count; particle++)
            {
                _particleSystems[particle].Update(gameTime);
                if (!_particleSystems[particle].IsComplete) continue;
                _particleSystems.RemoveAt(particle);
                particle--;
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            foreach (var e in _particleSystems)
            {
                spritebatch.Draw(e);
            }
        }
    }
}
