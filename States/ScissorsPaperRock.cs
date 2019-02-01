using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AudioMarcoPolo.States
{   
    
    public class ScissorsPaperRock : GState
    {

        public Color Color;
        public static string Message =
            "Touch the screen and I'll say scissors, paper, rock. in that time, hold your finger on, the top for scissors. the middle for paper. and the bottom for rock.";

        private Vector2 LastLocation;
        private float Top;
        private float Bottom;
        private float Middle;

        private float CTop;
        private float CBottom;
        private float CMiddle;

        public const int SCISSORS = 0;
        public const int PAPER = 1;
        public const int ROCK = 2;

        public int ChoiceComputer;
        public int ChoicePlayer;
        public int State;
        public Tween SPRWait;
        public Vector2 DragFrom;
        public ScissorsPaperRock(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {
            Game.BackGroundTune.SetVolume(0f);
            DragFrom = Vector2.Zero;
            Game.Audio.Say(Message);
            Message = "Touch to start";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
            LastLocation = new Vector2(-10, -10);
            ChoicePlayer = -1;
            ChoiceComputer = -1;
            State = 0;
            SPRWait = new Tween(new TimeSpan(0, 0, 0, 1,500), 0, 1);
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {
            if (State >= 9 && State != 11 && DragFrom != a)
            {
                if (a.Y > b.Y)
                {
                    State = 11;
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
            
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            if (Game.KeyboardInput.TypedKey(Keys.Escape))
            {
                NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
            }
            else
            {
                if (Game.UnifiedInput.Action)
                {
                    LastLocation = Game.UnifiedInput.Location;
                }
                if (LastLocation != new Vector2(-10, -10))
                {
                    
                    //Ok were playing
                    if (LastLocation.Y < Height/3f)
                    {                                                
                        if (State < 3)
                        {                            
                            ChoicePlayer = SCISSORS;
                        }                        
                    }
                    else if (LastLocation.Y < (Height/3f)*2f)
                    {
                        if (State < 3)
                        {                            
                            ChoicePlayer = PAPER;
                        }                        
                    }
                    else
                    {
                        if (State < 3)
                        {                            
                            ChoicePlayer = ROCK;
                        }                        
                    }

                    Top = MathHelper.Clamp((ChoicePlayer == SCISSORS) ? Top + .01f : Top - .01f, 0, 1);
                    Middle = MathHelper.Clamp((ChoicePlayer == PAPER) ? Middle + 0.1f : Middle - .01f, 0, 1);
                    Bottom = MathHelper.Clamp((ChoicePlayer == ROCK) ? Bottom + 0.01f : Bottom - .01f, 0, 1);
                    if (State < 4)
                    {
                        CTop = MathHelper.Clamp(CTop - .01f, 0, 1);
                        CMiddle = MathHelper.Clamp(CMiddle - .01f, 0, 1);
                        CBottom = MathHelper.Clamp(CBottom - .01f, 0, 1);
                    }

                    if (State == 0)
                    {
                        ChoiceComputer = BaseGame.Random.Next(2);
                        State = 1;
                        SPRWait.Reset();
                        Game.Audio.Say("scissors");
                    }
                    else
                    {
                        SPRWait.Update(gameTime.ElapsedGameTime);
                        if (State == 1 && SPRWait.IsComplete)
                        {
                            SPRWait.Reset();
                            State = 2;
                            Game.Audio.Say("paper");
                        }
                        else if (State == 2 && SPRWait.IsComplete)
                        {
                            SPRWait.Reset();
                            State = 3;
                            Game.Audio.Say("rock");
                        }
                        else if (State == 3 && SPRWait.IsComplete)
                        {
                            SPRWait.Reset();
                            State = 4;
                            Game.Audio.Say("I went");
                        }
                        else if (State == 4 && SPRWait.IsComplete)
                        {                            
                            State = 5;
                            if (ChoiceComputer == SCISSORS)
                            {
                                Game.Audio.Say("scissors");
                            }
                            else if (ChoiceComputer == PAPER)
                            {
                                Game.Audio.Say("paper");
                            }
                            else
                            {
                                Game.Audio.Say("rock");
                            }                            
                        }
                        else if (State == 5 && SPRWait.IsComplete)
                        {
                            if (ChoiceComputer == SCISSORS)
                            {
                                CTop = MathHelper.Clamp(CTop + .01f, 0, 1);
                                CMiddle = MathHelper.Clamp(CMiddle - .01f, 0, 1);
                                CBottom = MathHelper.Clamp(CBottom - .01f, 0, 1);
                                if (CTop >= 1)
                                {
                                    State = 6;
                                }                                
                            }
                            else if (ChoiceComputer == PAPER)
                            {
                                CTop = MathHelper.Clamp(CTop - .01f, 0, 1);
                                CMiddle = MathHelper.Clamp(CMiddle + .01f, 0, 1);
                                CBottom = MathHelper.Clamp(CBottom - .01f, 0, 1);
                                if (CMiddle >= 1)
                                {
                                    State = 6;
                                }
                            }
                            else
                            {
                                CTop = MathHelper.Clamp(CTop - .01f, 0, 1);
                                CMiddle = MathHelper.Clamp(CMiddle - .01f, 0, 1);
                                CBottom = MathHelper.Clamp(CBottom + .01f, 0, 1);
                                if (CBottom >= 1)
                                {
                                    State = 6;
                                }
                            }
                        }
                        else if (State == 6)
                        {
                            SPRWait.Reset();
                            Game.Audio.Say("you went");
                            State = 7;
                        }
                        else if (State == 7 && SPRWait.IsComplete)
                        {
                            SPRWait.Reset();
                            if (ChoicePlayer == SCISSORS)
                            {
                                Game.Audio.Say("scissors");
                            }
                            else if (ChoicePlayer == PAPER)
                            {
                                Game.Audio.Say("paper");
                            }
                            else
                            {
                                Game.Audio.Say("rock");
                            }    
                            State = 8;
                        }
                        else if (State == 8 && SPRWait.IsComplete)
                        {
                            SPRWait = new Tween(new TimeSpan(0, 0, 0, 3), 0, 1);
                            if (ChoicePlayer == SCISSORS && ChoiceComputer == PAPER)
                            {
                                Game.Audio.Say("scissors cuts paper. you win");
                            }
                            else if (ChoicePlayer == PAPER && ChoiceComputer == ROCK)
                            {
                                Game.Audio.Say("paper covers rock. you win");
                            }
                            else if (ChoicePlayer == ROCK && ChoiceComputer == SCISSORS)
                            {
                                Game.Audio.Say("Rock smashes scissors, you win");
                            }
                            else if (ChoiceComputer == SCISSORS && ChoicePlayer == PAPER)
                            {
                                Game.Audio.Say("scissors cuts paper. i win");
                            }
                            else if (ChoiceComputer == PAPER && ChoicePlayer == ROCK)
                            {
                                Game.Audio.Say("paper covers rock. i win");
                            }
                            else if (ChoiceComputer == ROCK && ChoicePlayer == SCISSORS)
                            {
                                Game.Audio.Say("Rock smashes scissors. i win");
                            }
                            else
                            {
                                Game.Audio.Say("draw");
                            }
                            State = 9;
                            DragFrom = Game.UnifiedInput.DragFrom;
                        }
                        else if (State == 9 && SPRWait.IsComplete)
                        {
                            SPRWait = new Tween(new TimeSpan(0, 0, 0, 1,500), 0, 1);
                            ChoicePlayer = -1;
                            Game.Audio.Say("Swipe up to play again, Down to leave");
                            State = 10;
                        }
                        else if (State == 10)
                        {
                            
                        }
                        else if (State == 11 && SPRWait.IsComplete)
                        {
                            SPRWait.Reset();
                            State = 0;
                        }
                    }
                }
            }
            return NextComponent ?? this;
        }

        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);

            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle(0,0,(int)(Width/2f),(int)(Height/3f)), ((Color==Color.Black)?Color.White:Color.Black) * Top);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle(0, (int)(Height / 3f), (int)(Width / 2f), (int)(Height / 3f)), ((Color == Color.Black) ? Color.White : Color.Black) * Middle);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle(0, (int)((Height / 3f)*2f), (int)(Width / 2f), (int)(Height / 3f)), ((Color == Color.Black) ? Color.White : Color.Black) * Bottom);


            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle((int)(Width / 2f), 0, (int)(Width / 2f), (int)(Height / 3f)), ((Color == Color.Black) ? Color.White : Color.Black) * CTop);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle((int)(Width / 2f), (int)(Height / 3f), (int)(Width / 2f), (int)(Height / 3f)), ((Color == Color.Black) ? Color.White : Color.Black) * CMiddle);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle((int)(Width / 2f), (int)((Height / 3f) * 2f), (int)(Width / 2f), (int)(Height / 3f)), ((Color == Color.Black) ? Color.White : Color.Black) * CBottom);


            SpriteBatch.End();
        }
    }
}
