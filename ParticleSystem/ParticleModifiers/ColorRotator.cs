using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.ParticleSystem.ParticleModifiers
{
    public class ColorRotator : IParticleModifier
    {
        private readonly List<Color> _colors;
        private readonly Tween _tween;
        private int _current;

        private int Next
        {
            get
            {
                if (_current + 1 >= _colors.Count) return 0;
                return _current + 1;
            }
        }
        public ColorRotator(TimeSpan time, params Color[] colors)
        {
            _colors = new List<Color>(colors);
            if (_colors.Count == 0)
            {
                _colors.Add(Color.White);
            }
            _tween = new Tween(time, 0f, 1f);
        }

        public void Update(GameTime gameTime, Particle p)
        {
            _tween.Update(gameTime.ElapsedGameTime);
            if (_tween.IsComplete)
            {
                _current++;
                if (_current >= _colors.Count) _current = 0;
                _tween.Reset();
            }
            p.Color = Color.Lerp(_colors[_current], _colors[Next], _tween);
        }
    }
}
