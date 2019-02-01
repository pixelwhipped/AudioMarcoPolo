using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AudioMarcoPolo.Audio;
using AudioMarcoPolo.Audio.Oscillators;
using AudioMarcoPolo.Content;
using AudioMarcoPolo.Input;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace AudioMarcoPolo.States
{
    public class Pong: GState
    {
        public Color Color;
        public static string Message =
            "Listen to the ball. the higher the Frequency the higher the ball will be, the louder the closer.";
        private SynthVoice voice1;
        private SynthInstrument instrument;

        public Vector2 DragFrom;
        public double State;
        public Tween Wait;
        public string SwipeMSG = "Swipe up to play again, Down to leave";

        public Rectangle paddle1;
        public Rectangle paddle2;
        public Rectangle ball;
        
        public bool p1movesUp, p1movesDown, p2movesUp, p2movesDown;
        
        public float p1p;                           //float that will store player 1 score
        public float p2p;                           //float that will store player 2 score
        public int RandoMin = 1;                            //Those 2 random integers are used to randomize ball directions
        public int RandoMax = 3;                            //in the Randomize() method to avoid repetition of ball movement
        public float Xspeed = -2;                   //Beginning Initial speed
        public float Yspeed = 1;
        public bool TwoPlayer;
        public bool IsLeft;
        public bool IsRight;

        public int PaddleWidth = 200;
        public Pong(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {            
            DragFrom = Vector2.Zero;
            Game.BackGroundTune.SetVolume(0f);
            if (Message == "Listen to the ball")
            {
                Wait = new Tween(new TimeSpan(0, 0, 0, 1,500), 0, 1);
            }
            else
            {
                Wait = new Tween(new TimeSpan(0, 0, 0, 7), 0, 1);
            }
            State = 0;
            Game.Audio.Say(Message);
            Message = "Listen to the ball";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;            
            voice1 = new SynthVoice(Game.Audio, AudioChannels.Synth);
            instrument = new SynthInstrument();            
            instrument.Oscillators.Add(new SineOscillator());
            instrument.Frequency = 220;
            voice1.LoadInstrument(instrument);
            Restart();
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {
            if (State > 5 && State != 7 && DragFrom != a)
            {
                if (a.Y > b.Y)
                {
                    State = 7;
                }
                else
                {
                    Game.Audio.Say("");
                    NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);                    
                }
            }
        }

        public override void OnTap(Vector2 a)
        {
            if (State == 0)
            {
                Game.Audio.Say("");
                Wait.Finish();
            }
            if (State == 1.5 || State == 1.6)
            {
                Wait.Finish();
                State = 2;
            }
        }

        public float sr;
        public float sl;
        public override IGState Update(GameTime gameTime, BaseGame grid)
        {
            
            if (Game.KeyboardInput.TypedKey(Keys.Escape))
            {
                NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                voice1.StopNote();
            }
            else
            {
                if (State == 1 || State ==1.4 || IsLeft)
                {
                    sl = MathHelper.Clamp(sl + 0.05f, 0f, 1f);
                }
                else if (State == 1.5 || State == 1.6 || IsRight)
                {
                    sr = MathHelper.Clamp(sr + 0.05f, 0f, 1f);
                }

                sl = MathHelper.Clamp(sl - 0.025f, 0f, 1f);
                sr = MathHelper.Clamp(sr - 0.025f, 0f, 1f);
                
                Wait.Update(gameTime.ElapsedGameTime);
                if (State == 0 && Wait.IsComplete)
                {
                    Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                    State = 1;
                }
                else if (State == 1 && Wait.IsComplete)
                {
                    Game.Audio.Say("Tap Left for 2 Players");
                    Wait = new Tween(new TimeSpan(0, 0, 0, 3), 0, 1);
                    State = 1.4;
                }
                else if (State == 1.4 && Wait.IsComplete)
                {                    
                    State = 1.5;
                }
                else if (State == 1.5 && Wait.IsComplete)
                {
                    Game.Audio.Say("Or Right for 1 Player");
                    Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                    State = 1.6;
                }
                else if (State == 1.6 && Wait.IsComplete)
                {
                    Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                    State = 2;
                }
                else if (State == 2 && Wait.IsComplete)
                {
                    if (Game.UnifiedInput.Action && Game.UnifiedInput.Location.X > Width/2f)
                    {
                        TwoPlayer = false;
                        State = 3;
                        Game.Audio.Say("Go, Get Ready " + ((Xspeed>0)?"Player 1":"Player 2"));
                    }
                    else if (Game.UnifiedInput.Action)
                    {
                        TwoPlayer = true;
                        State = 3;
                        Game.Audio.Say("Go, Get Ready " + ((Xspeed > 0) ? "Player 1" : "Player 2"));
                    }
                }
                else if (State == 3 && Wait.IsComplete)
                {
                    if (ball.Center.X < Width/2f)
                    {
                        IsLeft = true;
                        IsRight = false;
                    }
                    else
                    {
                        IsLeft = false;
                        IsRight = true;
                    }
                    var pan = 1f - ((ball.X/Width)*2f);
                    var mf = Height;
                    var cf = ball.Center.Y;
                    var mv = Width/2f;
                    float cv;
                    if (ball.Center.X < (Width/2f))
                    {
                        cv = mv - ball.Center.X;
                    }
                    else
                    {
                        cv = ball.Center.X - (Width/2f);
                    }

                    voice1.PlaySound(Math.Max(25f, (1f - (cf/mf))*500f), pan, (cv/mv));
                    MoveBall(gameTime); //Moves the ball
                    CheckScore(); //Check if one player scored
                    CheckIfMoving();
                    CheckInput();
                }
                else if (State == 4)
                {
                    if (ball.X < 1)
                    { p2p += 1; State = 7; }
                    else if (ball.X > Width)
                    { p1p += 1; State = 7; }

                    voice1.StopNote();
                    IsLeft = false;
                    IsRight = false;
                    DragFrom = Game.UnifiedInput.DragFrom;
                    
                    if (ball.X < 1)
                    {
                        Game.Audio.Say("Point Right Player " + p1p + " verse " + p2p);
                    }
                    else
                    {
                        Game.Audio.Say("Point Left Player" + p2p + " verse " + p1p);
                    }
                    
                    Wait = new Tween(new TimeSpan(0, 0, 0, 2, 500), 0, 1);
                    State = 5;
                }
                else if (State == 5 && Wait.IsComplete)
                {
                    Wait = new Tween(new TimeSpan(0, 0, 0, 2, 500), 0, 1);
                    Game.Audio.Say("swipe up to play, down to leave");
                    State = 6;
                }
                else if (State == 6)
                {
                }
                else if (State == 7 && Wait.IsComplete)
                {
                    

                    Restart();
                    State = 3;
                    Wait.Finish();
                }
            }
            return NextComponent ?? this;
        }

        public void CheckInput()
        {
            p1movesDown = Game.KeyboardInput.Pressed(Keys.A);            
            p1movesUp = Game.KeyboardInput.Pressed(Keys.Q);            
            p2movesDown = Game.KeyboardInput.Pressed(Keys.L);            
            p2movesUp = Game.KeyboardInput.Pressed(Keys.P);            
        }

        public void MoveBall(GameTime t1)
        {
            ball.X += (int)Xspeed;                                     //Changes ball coordinates based on speed in both x & y axis
            ball.Y += (int)Yspeed;
            if (ball.Y > (Height - ball.Height) || ball.Y < 0)
            {
                Yspeed = -Yspeed; //If ball touch one of the Y bounds, it's y speed gets a change in sign, and ball rebounce
                Game.Audio.Play(Cues.Bounce2);
            }
            if (ball.X > (Width) || ball.X < 1)
            {                
                Xspeed = -Xspeed; //Same for X bounds, with x speed
                
                
            }     
            if (ball.Intersects(paddle1) || ball.Intersects(paddle2))
            {

                int dst = paddle1.Y + PaddleWidth;
                int Distance = dst - ball.Y;
                Game.Audio.Play(Cues.Bounce1);
                if (Distance > (PaddleWidth*0.75f) || Distance < (PaddleWidth*0.25f)) { Randomize(); }  //If the ball intersects the paddle "away" from the centre, the ball movement get randomized
                else { Xspeed = -Xspeed; }                             //else, it's speed on the X axis gets simply reverted
                if (Xspeed > 0)
                {
                    Game.Audio.Say("The Balls going right");
                }
                else
                {
                    Game.Audio.Say("The Balls going left");
                }
            }
        }

        public override void Draw(GameTime gameTime, BaseGame grid)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);
           // SpriteBatch.Draw(BaseGame.Pixel, paddle2, (Color == Color.Black) ? Color.White : Color.Black);
          //  SpriteBatch.Draw(BaseGame.Pixel, paddle1, (Color == Color.Black) ? Color.White : Color.Black);
          //    SpriteBatch.Draw(BaseGame.Pixel, ball, (Color == Color.Black) ? Color.White : Color.Black);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle(0,0,(int) (Width/2f),(int) Height), ((Color == Color.Black) ? Color.White : Color.Black)*sl);
            SpriteBatch.Draw(BaseGame.Pixel, new Rectangle((int)(Width / 2f), 0, (int)(Width / 2f), (int)Height), ((Color == Color.Black) ? Color.White : Color.Black) * sr);
            //SpriteBatch.DrawString(Fonts.ArialSmall, "" + (cv/mv), new Vector2(10, 20), Color.Blue);

            

            SpriteBatch.End();
        }

        public void CheckIfMoving()                      //If player press the key to move the paddle, this method
        {                                                       //changes the Y position of the paddle Accordingly            
            if (p1movesUp && TwoPlayer)
            { paddle1.Y = paddle1.Y <= 0 ? 0 : paddle1.Y - 3; }
            else if (p1movesDown && TwoPlayer)
            { paddle1.Y = paddle1.Y >= (Height - paddle1.Height) ? (int)(Height - paddle1.Height) : paddle1.Y + 3; }
            else if (p2movesUp)
            { paddle2.Y = paddle2.Y <= 0 ? 0 : paddle2.Y - 3; }
            else if (p2movesDown)
            {paddle2.Y = paddle2.Y >= (Height - paddle1.Height) ? (int) (Height - paddle1.Height) : paddle2.Y + 3;}
            else
            {
                bool movedP1 = false;
                bool movedP2 = false;
                foreach (var t in TouchPanel.GetState())
                {
                    if (t.Position.X < (Width / 3f) && !movedP1 && !p1movesDown && !p1movesUp && TwoPlayer)
                    {
                        paddle1.Y = (int)MathHelper.Clamp(t.Position.Y - (paddle1.Height / 2f), 0, Height - paddle1.Height);
                        movedP1 = true;
                    }
                    else if (t.Position.X > (Width - (Width / 3f)) && !movedP2 && !p2movesDown && !p2movesUp)
                    {
                        paddle2.Y = (int) MathHelper.Clamp(t.Position.Y - (paddle2.Height/2f),0,Height-paddle2.Height);
                        movedP2 = true;
                    }
                }
                var m = Mouse.GetState();
                if (m.LeftButton == ButtonState.Pressed || m.MiddleButton == ButtonState.Pressed ||
                    m.RightButton == ButtonState.Pressed)
                {
                    if (m.X > (Width - (Width/3f)))
                    {
                        //paddle2.Y = (int)(m.Y - (paddle2.Height / 2f));
                        paddle2.Y = (int)MathHelper.Clamp(m.Y - (paddle2.Height / 2f), 0, Height - paddle2.Height);
                    }
                }

                if (!TwoPlayer) //Put in AI Here
                {
                    if (ball.X < Width/2f)
                    {
                        if(ball.Center.Y > paddle1.Center.Y + (paddle1.Height/2f))
                        {
                            { paddle1.Y = paddle1.Y >= (Height - paddle1.Height) ? (int)(Height - paddle1.Height) : paddle1.Y + 3; }
                        }else if (ball.Center.Y < paddle1.Center.Y - (paddle1.Height / 2f))
                        {
                            { paddle1.Y = paddle1.Y >= (Height - paddle1.Height) ? (int)(Height - paddle1.Height) : paddle1.Y - 3; }
                        }
                    }
                    
                }
            }
        }

        public void Restart()                            //Method called upon player scoring, to reset speed values
        {
            ball.Width = 16;        //and ball position
            ball.Height = 16;
            ball.X = (int) (Center.X-(ball.Width/2f)); Yspeed = 1;
            ball.Y = (int) (Center.Y-(ball.Height/2f)); RandoMin = 1;
            paddle1 = new Rectangle(14, (int)((Height / 2f) - (PaddleWidth/2f)), 20, PaddleWidth);
            paddle2 = new Rectangle((int)(Width - 34), (int)((Height / 2f) - (PaddleWidth/2f)), 20, PaddleWidth);
            RandoMax = 3;
            //IncreaseSpeed();
        }

        public void CheckScore()                         //Check if any player has scored, and increase p1p accordingly
        {
            if (ball.X < 1)
            { p2p += 1; State = 4; }
            else if (ball.X > Width)
            { p1p += 1; State = 4; }
        }

        public void IncreaseSpeed()                      //Increase both the normal speed and the results of
        {                                                       //any possible randomization in the Randomize() method
            RandoMin += 1;
            RandoMax += 1;
            Xspeed = Xspeed < 0 ? Xspeed -= 1 : Xspeed += 1;
        }

        public void Randomize()
        {
            float s = BaseGame.Random .Next(RandoMin, RandoMax);                     //Uses RandoMin & RandoMax values to randomize the X speed of the ball
            Xspeed = ball.Intersects(paddle1) ? Xspeed = s : Xspeed = -s;

            if (Yspeed < 0)                                            //If ball is moving upward, (so y speed is negative) the random value assigned
            {                                                          //will be changed in sign, so the ball can still go upward
                float t = BaseGame.Random.Next(RandoMin, RandoMax);
                Yspeed = -t;
            }
            else                                                       //Else, directly change the Y speed to a positive value
            { Yspeed = BaseGame.Random.Next(RandoMin, RandoMax); }
        }   

    }
}
