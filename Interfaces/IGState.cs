using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AudioMarcoPolo.Input;
using AudioMarcoPolo.UI;

namespace AudioMarcoPolo.Interfaces
{
    public interface IGState
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

        Settings Settings { get; }

        GamePersistance<GameData> GameData { get; }        

        float Transition { get; set; }        

        bool HasPrevious { get; }
        IGState NextComponent { get; set; }

        void OnTap(Vector2 a);
        void OnDragged(Vector2 a, Vector2 b);
        void Back();
    }
}
