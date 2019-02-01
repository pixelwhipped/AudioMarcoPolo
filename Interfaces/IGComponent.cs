using AudioMarcoPolo.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.Interfaces
{
    public interface IGComponent
    {
        KeyboardInput KeyboardInput { get; }
        UnifiedInput UnifiedInput { get; }
        Vector2 Center { get; }
        float Width { get; }
        float Height { get; }
        BaseGame Game { get; }
        SpriteBatch SpriteBatch { get; }
        bool IsVisible { get; set; }
        IGState Update(GameTime gameTime, BaseGame game);
        void Draw(GameTime gameTime, BaseGame game);
    }
}
