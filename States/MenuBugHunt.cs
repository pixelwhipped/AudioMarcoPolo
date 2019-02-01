using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using AudioMarcoPolo.Audio;
using AudioMarcoPolo.Audio.Oscillators;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;

namespace AudioMarcoPolo.States
{
    public class MenuBugHunt : GState
    {
        public Color Color;
        public static string Message = "Bug Hunt";
        public MenuBugHunt(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {
            Game.Audio.Say(Message);
            Message = "Bug Hunt";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {            
            if (Vector2.Distance(a, b) > 200)
            {
                if (a.X < b.X)
                {
                    NextComponent = new MenuPong(Game, null,
                        (Color == Color.Black) ? Color.White : Color.Black);
                }
                else
                {
                    NextComponent = new MenuScissorsPaperRock(Game, null,
                        (Color == Color.Black) ? Color.White : Color.Black);
                }
            }
        }

        public override void OnTap(Vector2 a)
        {
            NextComponent = new BugHunt(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            return NextComponent ?? this;
        }

        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);
            MainMenu.DrawLines(SpriteBatch, Width, Height, (Color == Color.Black) ? Color.White : Color.Black, new[]
            {
                "Bug Hunt",
                "Pesky bugs won't leave us along",
                "Swat the bugs by listening to the",
                "Audio Pan from Left and Right",
                "High Freqency indicates your close",
                "Low frequency means the bug is further",
                "Away"
            });
            SpriteBatch.End();
        }
    }
}
