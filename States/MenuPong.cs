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
    public class MenuPong : GState
    {
        public Color Color;
        public static string Message = "Pong, tap to play";
        public MenuPong(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {
            Game.Audio.Say(Message);
            Message = "Pong";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {            
            if (Vector2.Distance(a, b) > 200)
            {
                if (a.X < b.X)
                {
                    NextComponent = new MenuMarcoPolo(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                }
                else
                {
                    NextComponent = new MenuBugHunt( Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                }
            }
        }

        public override void OnTap(Vector2 a)
        {
            NextComponent = new Pong(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
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
                "Pong",
                "You can play 1 player using mouse, touch",
                "P an L Keys for up and down",
                "Two Players using either Touch and or Q and A keys",
                "for up and down",
                "Player 1 is to the Right and Player 2 to the Left",
                "The Audio Que's are Audio Pan left or Right",
                "To determin ball location horizontally",
                "Volume to determin Distance from center",
                "High frequency to Low Frequency indicates distance",
                "From the top of the screen"
            });
            SpriteBatch.End();
        }
    }
}
