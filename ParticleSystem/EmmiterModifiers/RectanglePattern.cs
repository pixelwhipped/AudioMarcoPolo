using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.ParticleSystem.EmmiterModifiers
{
    public class RectanglePattern : IEmitterModifier
    {
        private Vector2 _size;
        public RectanglePattern(Vector2 size)
        {
            _size = size;
        }
        public bool IsPattern { get { return true; } }
        public void Update(GameTime gameTime, Emitter e)
        {
            var halfWidth = _size.X / 2f;
            var halfHeight = _size.Y / 2f;
            var offset = Vector2.Zero;
            offset.X = (float)((halfWidth - (-halfWidth)) * BaseGame.Random.NextDouble() + (-halfWidth));
            offset.Y = (float)((halfHeight - (-halfHeight)) * BaseGame.Random.NextDouble() + (-halfHeight));

            e.EmissionPoint = Vector2.Add(e.EmitterLocation, offset);
        }
    }
}
