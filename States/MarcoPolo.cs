using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using AudioMarcoPolo.Content;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AudioMarcoPolo.States
{
    public class MarcoPolo: GState
    {
        public Color Color;

        public static string Message =
            "Try to find me by touching the screen. i'll let you know how close you r. Press Escape to leave.";

        private bool Marco;
        private Vector2 Location;
        private Tween Timer;
        private Tween Wait;
        private int state;
        private Vector2 Tap;
        private float lastDistance = -1f;
        private Tween TapTimer;

        public float HotD = .099f;
        public float WarmD = .2f;
        public float ColdD = .325f;
        public float CloseD = .11f;

        public Tween woosh;
        private float pan;
        public Vector2 DragFrom;
        public MarcoPolo(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {
            DragFrom = Vector2.Zero;
            Game.BackGroundTune.SetVolume(0f);
            woosh = new Tween(new TimeSpan(0,0,0,5),0,1);
            Game.Audio.Say(Message);
            Message = "Let's go";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
            Location = new Vector2(-10, -10);
            Tap = Location;
            Timer = new Tween(new TimeSpan(0, 0, 0, (Message=="Let's go")?3:10), 0, 1);
            Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            Marco = true;
            TapTimer = new Tween(new TimeSpan(0, 0, 0, 5), 0, 1);
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {
            if (state >= 3 && state != 5 && DragFrom != a)
            {
                if (a.Y > b.Y)
                {
                    state = 5;
                }
                else
                {
                    NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                    Game.Audio.Say("");
                }
            }
        }

        public override void OnTap(Vector2 a)
        {
            if (state==0)Timer.Finish();
            TapTimer.Reset();
            Tap = a;
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            Timer.Update(gameTime.ElapsedGameTime);
            Wait.Update(gameTime.ElapsedGameTime);
            if (Game.KeyboardInput.TypedKey(Keys.Escape))
                NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
            if (Timer.IsComplete)
            {
                TapTimer.Update(gameTime.ElapsedGameTime);
                if (TapTimer.IsComplete)
                {
                    TapTimer.Reset();
                    if (Location != new Vector2(-10, -10))
                    {
                        if (Marco)
                        {
                            Game.Audio.Say("Marco", pan);
                        }
                        else
                        {
                            Game.Audio.Say("Polo",pan);
                        }
                    }
                    Marco = !Marco;
                }
                if (state > 0 && state < 3)
                {
                    woosh.Update(gameTime.ElapsedGameTime);
                    if (woosh.IsComplete)
                    {
                        woosh.Reset();
                        game.Audio.Play(Cues.Swish);
                    }
                }
                switch (state)
                {
                    case 0:
                    {
                        Location = new Vector2(BaseGame.Random.Next((int)Width), BaseGame.Random.Next((int)Height));
                        pan = MathHelper.Clamp((Location.X/(Width/2f)) - 1f, -.8f, .8f);
                        state = 1;
                        break;
                    }
                    case 1:
                    {
                        Tap = new Vector2(-10, -10);
                        
                        state = 2;
                        break;
                    }
                    case 2:
                    {
                        if (Tap != new Vector2(-10, -10))
                        {
                            var d = VectorHelpers.Hypot(Location, Tap);//Math.Abs(Vector2.Distance(Location, Tap));
                            var m = VectorHelpers.Hypot(Vector2.Zero, new Vector2(Width, Height));//Math.Abs(Vector2.Distance(new Vector2(0, 0), new Vector2(Width, Height)));

                            var p = d/m;
                            if (lastDistance == -1f) lastDistance = p;
                            var c = MathHelper.Distance(p, lastDistance);
                            if (d < 40f)
                            {
                                lastDistance = -1f;
                                game.Audio.Say("You Got Me");
                                Wait.Reset();
                                state = 3;
                            }
                            else
                            {
                            //If within proximity of last the hotter colder warmer
                                if (c < CloseD)
                                {
                                    if (p < HotD)
                                    {
                                        if (p < lastDistance)
                                        {
                                            if (lastDistance < HotD)
                                            {
                                                game.Audio.Say("hotter", pan);
                                            }
                                            else
                                            {
                                                game.Audio.Say("hot", pan);
                                            }
                                        }
                                        else
                                        {
                                            game.Audio.Say("hot", pan);
                                        }
                                    }
                                    else if (p < WarmD)
                                    {
                                        if (p < lastDistance)
                                        {
                                            if (lastDistance < WarmD)
                                            {
                                                game.Audio.Say("warmer", pan);
                                            }
                                            else
                                            {
                                                game.Audio.Say("warm", pan);
                                            }
                                        }
                                        else
                                        {
                                            game.Audio.Say("warm", pan);
                                        }
                                    }
                                    else if (p < ColdD)
                                    {
                                        if (p < lastDistance)
                                        {
                                            game.Audio.Say("cooler", pan);
                                        }
                                        else
                                        {
                                            game.Audio.Say("cold", pan);
                                        }
                                    }
                                    else
                                    {
                                        game.Audio.Say("freezing", pan);
                                    }
                                }
                                else
                                {
                                    lastDistance = -1f;
                                    if (p < HotD)
                                    {
                                        game.Audio.Say("hot", pan);
                                    }
                                    else if (p < WarmD)
                                    {
                                        game.Audio.Say("warm", pan);
                                    }
                                    else if (p < ColdD)
                                    {
                                        game.Audio.Say("cold", pan);
                                    }
                                    else
                                    {
                                        game.Audio.Say("freezing", pan);
                                    }
                                }


                                lastDistance = p;
                            }
                        }
                        Tap = new Vector2(-10, -10);     
                        break;
                    }
                    case 3:
                    {
                        Wait = new Tween(new TimeSpan(0, 0, 0, 1,500), 0, 1);                         
                        Game.Audio.Say("Swipe up to play again, Down to leave");
                        DragFrom = Game.UnifiedInput.DragFrom;
                        state = 4;
                        break;
                    }
                    case 4:
                    {
                        break;
                    }
                    case 5:
                    {
                        Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                        state = 0;

                        if (Wait.IsComplete)
                        {
                            NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);

                        }
                        break;
                    }                
                    default:
                    {
                        state = 0;
                        break;
                    }
                }
            }
            return NextComponent ?? this;
        }

        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);
            //SpriteBatch.Draw(BaseGame.Pixel, new Rectangle((int)Location.X-1,(int)Location.Y-1,3,3), (Color==Color.Black)?Color.White:Color.Black);
            SpriteBatch.End();
        }
    }
}
