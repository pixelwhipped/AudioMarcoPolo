using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using AudioMarcoPolo.Audio;
using AudioMarcoPolo.Audio.Oscillators;
using AudioMarcoPolo.Content;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;
using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.States
{
    public class MainMenu : GState
    {
        public Color Color;

        public static string Message =
            "Welcome to Audio Marco Polo. Swipe left or right to select a game,  Tap the screen to play.";
        public MainMenu(BaseGame game, IGState previous,Color c)
            : base(game, previous)
        {
            Game.Audio.Say(Message);
            Message = "Select a game, Tap to play.";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
            Game.BackGroundTune.SetVolume(1f);
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {
            if (Vector2.Distance(a, b) > 200)
            {
                if (a.X < b.X)
                {
                    NextComponent = new MenuScissorsPaperRock(Game, null, (Color == Color.Black) ? Color.White : Color.Black);

                }
                else
                {
                    NextComponent = new MenuMarcoPolo(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                }
            }
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            return NextComponent ?? this;
        }

        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);
            DrawLines(SpriteBatch, Width, Height, (Color == Color.Black) ? Color.White : Color.Black, new[]
            {
                "Main Menu",
                "Swipe Left or Right to select a game",
                "Tap the Screen to Play",
                "At any time during a game you can press",
                "ESC to return to the Main Menu",
                "After a game or turn you can swipe up to play again",
                "Or down to leave that game",
                "For Advice or Suggestions",
                "Contact PixelWhipped@outlook.com"
            });
            SpriteBatch.End();
        }

        public static void DrawLines(SpriteBatch batch, float width, float height, Color c,string[] lines)
        {
            float mw = 0f;
            float h = 0;
            foreach (var line in lines)
            {
                var w = Fonts.ArialLarge.MeasureString(line);
                mw = Math.Max(mw, w.X);
                h += w.Y + 4;
            }
            var ch =  (height/2f) - (h/2f);
            
            foreach (var line in lines)
            {
                var w = Fonts.ArialLarge.MeasureString(line);
                batch.DrawString(Fonts.ArialLarge,line,new Vector2((width/2f)-(w.X/2f),ch),c);
                ch += w.Y + 4;
            }
        }
    }
}
