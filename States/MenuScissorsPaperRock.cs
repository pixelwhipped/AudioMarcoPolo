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
    public class MenuScissorsPaperRock : GState
    {
        public Color Color;
        public static string Message = "Scissors Paper Rock";
        public MenuScissorsPaperRock(BaseGame game, IGState previous,Color c)
            : base(game, previous)
        {
            Game.Audio.Say(Message);
            Message = "Scissors Paper Rock";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {            
            if (Vector2.Distance(a, b) > 200)
            {
                if (a.X < b.X)
                {
                    NextComponent = new MenuBugHunt(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                }
                else
                {
                    NextComponent = new MenuMarcoPolo( Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                }
            }
        }

        public override void OnTap(Vector2 a)
        {
            NextComponent = new ScissorsPaperRock(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
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
                "Scissors Paper Rock",
                "Top play when the game starts",
                "Touch or Hold Top for Scissors",
                "Middle for Paper and Bottom for Rock",
            });
            SpriteBatch.End();
        }
    }
}
