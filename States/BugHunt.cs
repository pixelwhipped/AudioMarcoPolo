using System;
using System.Collections.Generic;
using System.Linq;
using AudioMarcoPolo.Audio;
using AudioMarcoPolo.Audio.Oscillators;
using AudioMarcoPolo.Content;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AudioMarcoPolo.States
{
    public struct Splatter
    {
        public Texture2D tex;
        public Vector2 loc;
    }

    public class BugHunt : GState
    {

        public string[] splatAcr = new[] {"splat", "ewe", "messy", "splat","squish","splat"};
        public Color Color;
        public static string Message =
            "Get that annoying bug, it keeps making noise. Press Escape to leave.";

        private Vector2 LastLocation;
        private float Top;
        private float Bottom;
        private float Middle;
        private SynthVoice voice1;
        private SynthInstrument instrument;
        //public Bug Bug;
        public Vector2 enemy;
        
        private Vehicle[] cars;
        private Vehicle otherCar;
        private SB SBs;
        private SB steeringBehaviour;
        public bool identicalBehaviour;
        public bool behaviourChanged;
        public float Difficulty;
        public Vector2 DragFrom;

        public int State;
        public Tween Wait;
        public List<Splatter> Splats;
        public string SwipeMSG = "Swipe up to play again, Down to leave, It will get harder from here on";
        public BugHunt(BaseGame game, IGState previous, Color c)
            : base(game, previous)
        {
            
            DragFrom = Vector2.Zero;
            Game.BackGroundTune.SetVolume(0f);
            Splats = new List<Splatter>();
            if (Message == "Get That Bug")
            {
                Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            }
            else
            {
                Wait = new Tween(new TimeSpan(0, 0, 0, 5), 0, 1);    
            }            
            State = 0;
            Game.Audio.Say(Message);
            Message = "Get That Bug";
            game.ParticleColor = (c == Color.Black) ? Color.White : Color.Black;
            Color = c;
            LastLocation = new Vector2(-10, -10);
            voice1 = new SynthVoice(Game.Audio,AudioChannels.Synth);
            instrument = new SynthInstrument();
            //instrument.Oscillators.Add(new SquareOscillator());
            instrument.Oscillators.Add(new SawtoothOscillator());
            instrument.Frequency = 220; // frequence de base
            voice1.LoadInstrument(instrument);
            Difficulty = 200f;
            enemy = (Game.UnifiedInput.Location==Vector2.Zero)?enemy:Game.UnifiedInput.Location;

            steeringBehaviour = SB.Flee;
            SBs = steeringBehaviour;
            cars = new Vehicle[1];
            InitializeCars();
        }

        //initialize all cars including "otherCar"
        private void InitializeCars()
        {
            otherCar = new Vehicle(Game.Bounds, SB.Wander, -2,enemy,new Vector2(4,4));
            otherCar.Mass = 0.5f;
            otherCar.MaxSpeed = 4;
            otherCar.CreateObstacles();
            for (int i = 0; i < cars.Length; i++)
            {
                var c = new Vehicle(Game.Bounds, SBs, i + 1, enemy, new Vector2(4, 4));
                c.MaxSpeed = 4;
                c.Mass = Difficulty; //Alter to make harder
                cars[i] = c;
                //if (!identicalVehicles)
                //    Thread.Sleep(50);
            }
            Vehicle.SetCarsData(ref cars);
        }

        public Vehicle FirstVehicle
        {
            get
            {
                return cars[0];
            }
        }

        public void Step(ref Vector2 targetPosistion)
        {            
            SetSameBehaviour();
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i].Step(ref targetPosistion);
                var v = new Vector2(0f, 0f);
                cars[i].Step(ref v);
                v = new Vector2(Width, 0f);
                cars[i].Step(ref v);
                v = new Vector2(Width, Height);
                cars[i].Step(ref v);
                v = new Vector2(0f, Height);
                cars[i].Step(ref v);
            }
            if (steeringBehaviour == SB.CF || steeringBehaviour == SB.FCS || steeringBehaviour == SB.FCAS)
            {
                otherCar.Step(ref targetPosistion);
                targetPosistion = otherCar.CurrentPosition;
            }
        }
        //when behaviour changes, should it affect all vehicles?
        private void SetSameBehaviour()
        {
            if (identicalBehaviour)
            {
                if (behaviourChanged)
                {
                    for (int i = 0; i < cars.Length; i++)
                    {
                        cars[i].SteeringBehaviour = steeringBehaviour;
                    }
                    behaviourChanged = false;
                }
            }
        }
        public Vector2 GetTarget()
        {

            return (Game.UnifiedInput.Location == Vector2.Zero) ? enemy : Game.UnifiedInput.Location;
        }

        public override void OnDragged(Vector2 a, Vector2 b)
        {
            if (State >= 2 && State != 4 && DragFrom != a)
            {
                if (a.Y > b.Y)
                {
                    State = 4;
                    enemy = b;
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
            if (State == 0)
            {
                Game.Audio.Say("");
                Wait.Finish();
            }
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            if (Game.KeyboardInput.TypedKey(Keys.Escape))
            {
                NextComponent = new MainMenu(Game, null, (Color == Color.Black) ? Color.White : Color.Black);
                voice1.StopNote();
            }
            else
            {
                Wait.Update(gameTime.ElapsedGameTime);
                if (State == 0 && Wait.IsComplete)
                {
                    enemy = GetTarget();
                    Step(ref enemy); // .Update(gameTime);
                    var md = VectorHelpers.Hypot(Vector2.Zero, new Vector2(Width, Height));
                    var cd = VectorHelpers.Hypot(FirstVehicle.CurrentPosition, enemy);
                    var pan = 1f - ((FirstVehicle.CurrentPosition.X/Width)*2f);
                    voice1.PlaySound(Math.Max(25f, (1f - (cd/md))*500f), pan);
                        //(Game.UnifiedInput.Location.X / Game.Width)*140);
                    if (cd < 90)
                    {
                        State = 1;
                        Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                        var s = BaseGame.Random.Next(3);
                        Texture2D t;
                        if (s == 0)
                        {
                            t = Textures.Splat1;
                        }else if (s == 1)
                        {
                            t = Textures.Splat2;
                        }
                        else
                        {
                            t = Textures.Splat3;
                        }
                        Splats.Add(new Splatter
                        {
                            loc = FirstVehicle.CurrentPosition-new Vector2(t.Width/2f,t.Height/2f),
                            tex = t
                        });
                    }
                }
                else if (State == 1)
                {
                    Wait.Reset();
                    Game.Audio.Play(Cues.Splat);
                    Game.Audio.Say(splatAcr[BaseGame.Random.Next(splatAcr.Count())]);
                    voice1.StopNote();
                    DragFrom = Game.UnifiedInput.DragFrom;
                    State = 2;

                }
                else if (State == 2 && Wait.IsComplete)
                {
                    Wait = new Tween(new TimeSpan(0, 0, 0, 2, 500), 0, 1);                    
                    Game.Audio.Say(SwipeMSG);
                    SwipeMSG = "swipe up to play, down to leave";
                    State = 3;
                }
                else if (State == 3)
                {
                }
                else if (State == 4 && Wait.IsComplete)
                {
                    Difficulty = MathHelper.Clamp(Difficulty - 5, 10, 100);
                    Wait = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                    var k = true;
                    while (k)
                    {
                        var cd = VectorHelpers.Hypot(FirstVehicle.CurrentPosition, enemy);

                        FirstVehicle.CurrentPosition = new Vector2(BaseGame.Random.Next((int) Width),
                            BaseGame.Random.Next((int) Height));
                        if (cd > 180)
                        {
                            k = false;
                        }
                    }
                    State = 0;

                    Wait.Finish();
                }
            }
            return NextComponent ?? this;
        }

        
        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, Color);
            foreach (var splatter in Splats)
            {
                SpriteBatch.Draw(splatter.tex, splatter.loc, (Color == Color.Black) ? Color.White : Color.Black);
            }
            SpriteBatch.End();
        }
    }
}
