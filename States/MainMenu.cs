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
    public class MenuSelection
    {
        public string Message;
        public Color Color;
        public Routine Action;         

        public MenuSelection(string message, Color color, Routine action)
        {
            Message = message;
            Color = color;
            Action = action;            
        }
    }

    public class SwipeMenu
    {
        private CyclicList<MenuSelection> _menuItems;

        public SwipeMenu()
        {
            _menuItems = new CyclicList<MenuSelection>();
        }

        public void Add(string message, Routine item)
        {
            Color c;
            if (_menuItems.Count == 0)
            {
                c = Color.White;
            }
            else
            {
                c = (_menuItems.Count%2 == 1) ? Color.White:Color.Black;
            }
            var m = new MenuSelection(message, c, item);
            _menuItems.Add(m);
        }

        public void Next()
        {
            _menuItems.Next();
        }

        public void Previous()
        {
            _menuItems.Previous();
        }
    }
    public class MainMenu : GState
    {
        private SynthVoice voice1;
        private SynthVoice voice2;
        private SynthVoice voice3;
        public Task TestSound()
        {
            return Task.Run(() =>
            {
                voice1 = new SynthVoice(Game.Audio,AudioChannels.Synth);
                //voice2 = new SynthVoice(Game.Audio,AudioChannels.Synth);
                //voice3 = new SynthVoice(Game.Audio, AudioChannels.Synth);

                SynthInstrument instrument = new SynthInstrument();
                instrument.Oscillators.Add(new SineOscillator());
                instrument.Oscillators.Add(new SquareOscillator());
                instrument.Frequency = 440f;//440f; // frequence de base
                voice1.LoadInstrument(instrument);
                //voice2.LoadInstrument(instrument);
                //voice3.LoadInstrument(instrument);
                voice1.PlaySound(140); //do
                //voice2.PlaySound(164.81f);//mi
                //voice2.PlaySound(196f);               
                
            });
        }

        private bool IsPlaying;

        

        
        public MainMenu(BaseGame game, IGState previous)
            : base(game, previous)
        {
            UnifiedInput.DraggedListeners.Add(ChangeMenu);

            _menuItems.Add(new MenuSelection("Swipe left or right to select a game,  Tap the screen to play.", () => { },
                _menuItems));
            _menuItems.Reset();
            Game.Audio.Say(_menuItems.Value.Message);
        }

        private void ChangeMenu(Vector2 a, Vector2 b)
        {
            if (Vector2.Distance(a, b) > 200)
            {
                Game.Audio.Say("Hello");
            }
        }

        private void Tap(Vector2 value)
        {
            /*
            if (!IsPlaying)
            {
                IsPlaying = true;
                TestSound();
            }
            else
            {
                IsPlaying = false;               
                voice1.StopNote();
               // voice2.StopNote();
               // voice3.StopNote();
            }
            */
        }

        public override IGState Update(GameTime gameTime, BaseGame game)
        {
            return NextComponent ?? this;
        }

        public override void Draw(GameTime gameTime, BaseGame game)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BaseGame.Pixel, Game.Bounds, _menuItems.Value.Color);
            SpriteBatch.End();
        }
    }
}
